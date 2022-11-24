using System;

namespace StructuringNotifications.Interop.OperationsApi.Dto;

public class DeliveryGidcDto
{
    public string SalesOrder { get; set; } = null!;
    public string Document { get; set; } = null!;
    public DateTime GoodsIssueDate { get; set; }
    public string MaterialDescription { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public decimal BsAmount { get; set; }
}
