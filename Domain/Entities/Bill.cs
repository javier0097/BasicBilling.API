using BasicBilling.API.Domain.Enums;

namespace BasicBilling.API.Domain.Entities;

public class Bill
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public ServiceType ServiceType { get; set; }
    public string Period { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Pending";

    public Client Client { get; set; } = null!;
    public Payment? Payment { get; set; }
}
