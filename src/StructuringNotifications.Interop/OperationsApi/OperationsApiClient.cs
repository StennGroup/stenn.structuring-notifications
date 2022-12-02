using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Operations.Contracts.Dto;
using StructuringNotifications.Interop.Abstractions;
using StructuringNotifications.Interop.OperationsApi.Dto;

namespace StructuringNotifications.Interop.OperationsApi;

public class OperationsApiClient : IOperationsApi
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _serializerOptions;

    public OperationsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _serializerOptions = new JsonSerializerOptions();
        _serializerOptions.Converters.Add(new JsonStringEnumConverter());
        _serializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    }

    public async Task<IReadOnlyCollection<InvoiceDto>> GetInvoicesListByDelayDaysAsync(int delayDays,
        string[] dunesToInclude,
        CancellationToken token)
    {
        var path = $"/api/odata/v1/invoices/Default.ByOverdueDaysAndSuppliers/?overdueDays={delayDays}&" +
                   $"suppliersToInclude={string.Join("%2C", dunesToInclude)}&" +
                   "$expand=deal($select=id,financingDate,investmentContractId)," +
                   "tradeRelation($select=id,sellerId,buyerId,doNotGenerateNotice,oneTimeNoticeId,sourceSystemId;" +
                   "$expand=seller($select=id,onboardingId,duns,name)," +
                   "buyer($select=id,onboardingId,duns,name,allowToSendOverdueInformation))";

        var data = await _httpClient.GetFromJsonAsync<OData<InvoiceDto>>(path, _serializerOptions, token);

        return data?.Value ?? new List<InvoiceDto>();
    }

    public async Task<IReadOnlyCollection<OwnBankAccountDto>> GetOwnBankAccountsAsync(CancellationToken token)
    {
        var data = await _httpClient.GetFromJsonAsync<OData<OwnBankAccountDto>>("/api/odata/v1/ownBankAccounts",
            _serializerOptions, token);

        return data != null
            ? data.Value
            : throw new Exception("Unable to get own bank accounts");
    }

    public async Task<IReadOnlyCollection<DealAttachmentLinkDto>> GetActualAttachmentsForDealsAsync(
        DealAttachmentType dealAttachmentType, Guid[] dealIds, CancellationToken token)
        => await PostJsonAsync<Guid[], List<DealAttachmentLinkDto>>(
               $"/api/v1/documents/getActualAttachmentsForDeals/{(int)dealAttachmentType}", dealIds, token)
           ?? new();


    public async Task<IReadOnlyCollection<DocumentLinkDto>> GetDocumentAttachmentLinksAsync(
        DocumentType documentType, Guid[] entityIds, CancellationToken token)
        => await PostJsonAsync<Guid[], List<DocumentLinkDto>>(
               $"/api/v1/documents/getDocumentAttachmentLinks/{(int)documentType}", entityIds, token)
           ?? new();

    public async Task<DeliveryGidcDto[]> GetFinancedGidc(DateTime date, CancellationToken token)
        => await _httpClient.GetFromJsonAsync<DeliveryGidcDto[]>(
               $"api/v1/invoices/gidcFinancedAtDate/{date:yyyy-MM-dd}", _serializerOptions, cancellationToken: token)
           ?? Array.Empty<DeliveryGidcDto>();


    private async Task<TOutput?> PostJsonAsync<TInput, TOutput>(string url, TInput postData, CancellationToken token)
    {
        var response = await _httpClient.PostAsJsonAsync(url, postData, cancellationToken: token);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync(token);
        return JsonSerializer.Deserialize<TOutput>(responseBody, _serializerOptions);
    }
}