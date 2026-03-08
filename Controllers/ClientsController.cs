using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BasicBilling.API.Application.DTOs;
using BasicBilling.API.Application.Features.Bills.Queries;
using BasicBilling.API.Application.Features.Payments.Queries;
using MediatR;

namespace BasicBilling.API.Controllers;

/// <summary>
/// Provides client billing and payment information.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ClientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all pending bills for a specific client.
    /// </summary>
    [HttpGet("{id}/pending-bills")]
    [ProducesResponseType(typeof(IEnumerable<BillDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPendingBills(int id)
    {
        var bills = await _mediator.Send(new GetPendingBillsQuery(id));
        return Ok(bills);
    }

    /// <summary>
    /// Retrieves the payment history for a specific client.
    /// </summary>
    [HttpGet("{id}/payment-history")]
    [ProducesResponseType(typeof(IEnumerable<PaymentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentHistory(int id)
    {
        var payments = await _mediator.Send(new GetPaymentHistoryQuery(id));
        return Ok(payments);
    }
}
