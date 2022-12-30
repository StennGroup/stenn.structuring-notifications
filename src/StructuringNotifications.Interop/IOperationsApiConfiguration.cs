namespace StructuringNotifications.Interop;

public interface IOperationsApiConfiguration
{
    public string RootUrlOperationsApi { get; set; }
    public string OperationsApiAdClientId { get; set; }
    public string OperationsApiAdClientSecret { get; set; }
    public string OperationsApiAdInstance { get; set; }
    public string OperationsApiAdTenantId { get; set; }
}