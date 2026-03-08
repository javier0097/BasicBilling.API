namespace BasicBilling.API.Application.DTOs;

public class BillDto
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public string ServiceType { get; set; } = null!;
    public string Period { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Status { get; set; } = null!;
}
