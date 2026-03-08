namespace BasicBilling.API.Domain.Entities;

public class Payment
{
    public int Id { get; set; }
    public int BillId { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }

    public Bill Bill { get; set; } = null!;
}
