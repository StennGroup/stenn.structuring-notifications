using Operations.Contracts.Dto;

namespace StructuringNotifications.Interop.Abstractions;

public interface IOperationsApi
{
    Task<IReadOnlyCollection<InvoiceDto>> GetInvoicesListByDelayDaysAsync(int delayDays, string[] dunsesToExclude,
        CancellationToken token);

    Task<IReadOnlyCollection<OwnBankAccountDto>> GetOwnBankAccountsAsync(CancellationToken token);

    Task<IReadOnlyCollection<DealAttachmentLinkDto>> GetActualAttachmentsForDealsAsync(
        DealAttachmentType dealAttachmentType, Guid[] dealIds, CancellationToken token);

    Task<IReadOnlyCollection<DocumentLinkDto>> GetDocumentAttachmentLinksAsync(
        DocumentType documentType, Guid[] entityIds, CancellationToken token);
}