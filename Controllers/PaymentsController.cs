using Microsoft.AspNetCore.Mvc;
using BasicBilling.API.Application.DTOs;
using BasicBilling.API.Application.Features.Payments.Commands;
using MediatR;

namespace BasicBilling.API.Controllers;

/// <summary>
/// Handles bill payment processing.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
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
        var payment = await _mediator.Send(new ProcessPaymentCommand(dto));
        return Ok(payment);
    }
}
