using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Operations.Contracts.Dto;
using OperationsNotifications.Contracts.Events;
using OperationsNotifications.Contracts.Dto;
using Seedwork.Messaging;
using Seedwork.SystemDate;
using Serilog;
using Serilog.Context;
using StructuringNotifications.Interop.Abstractions;
using BankDetailsDto = OperationsNotifications.Contracts.Dto.BankDetailsDto;
using InvoiceDto = Operations.Contracts.Dto.InvoiceDto;
using MoneyDto = OperationsNotifications.Contracts.Dto.MoneyDto;


namespace StructuringNotifications.Application;

public class EssarOverdueNotificationService
{
    private readonly ISystemDate _systemDate;
    private readonly IOperationsApi _operationsApiClient;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    private readonly IMessageSender _messageSender;
    private readonly IOverdueNotificationsConfiguration _configuration;

    public EssarOverdueNotificationService(ISystemDate systemDate,
        IOperationsApi operationsApiClient,
        IMapper mapper,
        ILogger logger,
        IMessageSender messageSender,
        IOverdueNotificationsConfiguration configuration)
    {
        _systemDate = systemDate;
        _operationsApiClient = operationsApiClient;
        _mapper = mapper;
        _logger = logger;
        _messageSender = messageSender;
        _configuration = configuration;
    }

    public async Task SendOverdueNotificationEventsAsync(CancellationToken token)
    {
        var today = _systemDate.Today;
        LogContext.PushProperty("OverdueNotificationDate", today);

        // Don't send anything on Saturday and Sunday
        if (today.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            _logger.Debug("Skip overdue notifications sending due to weekend");
            return;
        }

        _logger.Debug("Starting Essar overdue notifications sending");

        var notifications = await GetNotifications(0, token);

        // On Monday send notifications for Saturday and Sunday
        if (today.DayOfWeek == DayOfWeek.Monday)
        {
            notifications.AddRange(await GetNotifications(1, token));
            notifications.AddRange(await GetNotifications(2, token));
        }

        _logger.Debug("Found {NotificationsCount} notifications to send", notifications.Count);

        foreach (var notification in notifications)
            _messageSender.Publish(notification);

        _logger.Debug("Essar overdue notification sending was finished");
    }

    private async Task<List<OverdueEvent>> GetNotifications(int switchDays, CancellationToken token)
        => await OverdueNotificationType.AvailableTypes.ToAsyncEnumerable()
            .SelectManyAwaitWithCancellation(async (type, innerToken) =>
                (await GetNotifications(switchDays, type, innerToken)).ToAsyncEnumerable())
            .ToListAsync(token);

    private async Task<IEnumerable<OverdueEvent>> GetNotifications(int switchDays,
        OverdueNotificationType notificationType,
        CancellationToken token)
    {
        var invoices =
            await _operationsApiClient.GetInvoicesListByDelayDaysAsync(notificationType.DelayDays + switchDays,
                _configuration.CompanyDunsesToNotify, token);


        if (!invoices.Any())
            return Enumerable.Empty<OverdueEvent>();

        var groupedInvoices = invoices
            .GroupBy(x => x.TradeRelation.SourceSystemId)
            .ToList();

        var notifications = new List<OverdueEvent>();
        foreach (var invoiceGroup in groupedInvoices)
        {
            try
            {
                var notification = await GetEvent(invoiceGroup, notificationType, token);
                if (notification is not null)
                    notifications.Add(notification);
            }
            catch (Exception e)
            {
                _logger.Error(e, "OverdueNotificationService: Unable to generate overdue notification " +
                                 "event for TradeRelation {TradeRelationSourceSystemId} Delay days: {DelayDays}",
                    invoiceGroup.Key, notificationType.DelayDays);
            }
        }

        return notifications;
    }

    private async Task<OverdueEvent?> GetEvent(IGrouping<string, InvoiceDto> groupedInvoice,
        OverdueNotificationType notificationType,
        CancellationToken token)
    {
        var tradeRelation = groupedInvoice.First().TradeRelation;
        if (tradeRelation.Buyer.AllowToSendOverdueInformation != true && !notificationType.SendToSupport)
            return null;

        var allInvoiceAttachments = (await _operationsApiClient.GetDocumentAttachmentLinksAsync(
                DocumentType.CommercialInvoice, groupedInvoice.Select(x => x.Id).ToArray(), token))
            .Where(x => x.FileSizeInBytes > 0)
            .ToList();

        var dealIds = groupedInvoice.Select(x => x.Deal.Id).Distinct().ToArray();
        var allDealAttachments = (await _operationsApiClient.GetActualAttachmentsForDealsAsync(
                DealAttachmentType.Notice, dealIds, token))
            .Where(x => x.FileSizeInBytes > 0)
            .ToList();

        var isAllDealAttachmentsAvailable = tradeRelation.DoNotGenerateNotice ||
                                            !string.IsNullOrWhiteSpace(tradeRelation.OneTimeNoticeId) ||
                                            dealIds.All(x => allDealAttachments.Any(y => y.DealId == x));

        var currencies = groupedInvoice
            .Select(x => x.RepaymentAmountNationalCurrency.Currency.IsoNumericCode)
            .Distinct()
            .ToArray();

        var bankDetails = await currencies.ToAsyncEnumerable()
            .SelectAwaitWithCancellation(GetOwnBankAccount)
            .ToListAsync(token);

        return CreateOverdueEvent(groupedInvoice, notificationType, allInvoiceAttachments, allDealAttachments,
            isAllDealAttachmentsAvailable, bankDetails);
    }

    private OverdueEvent CreateOverdueEvent(IGrouping<string, InvoiceDto> groupedInvoice,
        OverdueNotificationType notificationType, IReadOnlyCollection<DocumentLinkDto> allInvoiceAttachments,
        IEnumerable<DealAttachmentLinkDto> allDealAttachments, bool isAllDealAttachmentsAvailable,
        IEnumerable<OwnBankAccountDto> bankDetails)
    {
        return new OverdueEvent
        {
            TradeRelationSourceSystemId = groupedInvoice.Key,
            DaysOverdue = notificationType.DelayDays,
            SendToBuyer = true,
            SendToBuyerSignatureAddress = notificationType.SendToSignatureAddress,
            SendToSupport = notificationType.SendToSupport,
            SendToSupplier = notificationType.SendToSupplier,
            Invoices = groupedInvoice
                .Select(x => new OperationsNotifications.Contracts.Dto.InvoiceDto
                {
                    InvoiceDocumentLink = allInvoiceAttachments
                        .Where(y => y.EntityId == x.Id)
                        .Select(y => _mapper.Map<LinkDto>(y))
                        .FirstOrDefault(),
                    DueDate = x.DueDate,
                    // ReSharper disable once PossibleInvalidOperationException
                    FinancingDate = x.Deal.FinancingDate ?? throw new Exception("No deal financing date"),
                    GraceDate = x.GraceDate,
                    Number = x.Number,
                    RepaymentAmountNationalCurrency =
                        _mapper.Map<MoneyDto>(x.RepaymentAmountNationalCurrency),
                    RepaymentAmountOutstandingNationalCurrency =
                        _mapper.Map<MoneyDto>(x.RepaymentAmountOutstandingNationalCurrency),
                    SupplyDate = x.SupplyDate,
                    ShipmentDate = x.ShipmentDate
                })
                .ToList(),
            NoticeDocuments = allDealAttachments.Select(x => new LinkDto
                {
                    FileName = x.FileName,
                    FileSizeInBytes = x.FileSizeInBytes,
                    Url = x.Url
                })
                .ToList(),
            IsAllNoticesAvailable = isAllDealAttachmentsAvailable,
            StennBankDetails = bankDetails.Select(x => new BankDetailsDto
            {
                Currency = x.Currency.Iso3LetterCode,
                BankName = x.BankName,
                BeneficiaryName = x.BankDetails.BeneficiaryName,
                Swift = x.BankDetails.Swift,
                Iban = x.Iban,
                BeneficiaryAddress1 = x.BankDetails.BeneficiaryAddress1,
                BeneficiaryAddress2 = x.BankDetails.BeneficiaryAddress2,
                BeneficiaryAddress3 = x.BankDetails.BeneficiaryAddress3,
            }).ToList()
        };
    }

    private IReadOnlyCollection<OwnBankAccountDto>? _ownBankDetails;

    private async ValueTask<OwnBankAccountDto> GetOwnBankAccount(int currencyCode, CancellationToken token)
    {
        _ownBankDetails ??= await _operationsApiClient.GetOwnBankAccountsAsync(token);

        return _ownBankDetails.First(x =>
            x.Currency.IsoNumericCode == currencyCode &&
            x.Program == OwnBankAccountProgram.Staf);
    }
}