using Microsoft.AspNetCore.Mvc;
using BasicBilling.API.Application.DTOs;
using BasicBilling.API.Application.Interfaces.Services;

namespace BasicBilling.API.Controllers;

/// <summary>
/// Handles bill payment processing.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>
    /// Processes a payment for an existing pending bill.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequestDto dto)
    {
        var payment = await _paymentService.ProcessPaymentAsync(dto);
        return Ok(payment);
    }
}
