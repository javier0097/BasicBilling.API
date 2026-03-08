namespace BasicBilling.API.Domain.Entities;

public class Client
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Bill> Bills { get; set; } = new List<Bill>();
}
