namespace BasicBilling.API.Application.DTOs;

public class PaymentDto
{
    public int Id { get; set; }
    public int BillId { get; set; }
    public string ServiceType { get; set; } = null!;
    public string Period { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string Status { get; set; } = null!;
}
