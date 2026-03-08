using System.ComponentModel.DataAnnotations;
using BasicBilling.API.Domain.Enums;

namespace BasicBilling.API.Application.DTOs;

public class CreateBillDto
{
    [Required]
    public int ClientId { get; set; }

    [Required]
    public ServiceType ServiceType { get; set; }

    [Required]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "Billing period must follow the YYYYMM format.")]
    public string Period { get; set; } = null!;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be a positive number.")]
    public decimal Amount { get; set; }
}
