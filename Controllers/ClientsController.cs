using Microsoft.AspNetCore.Mvc;
using BasicBilling.API.Application.DTOs;
using BasicBilling.API.Application.Interfaces.Services;

namespace BasicBilling.API.Controllers;

/// <summary>
/// Provides client billing and payment information.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ClientsController : ControllerBase
{
    private readonly IBillService _billService;
    private readonly IPaymentService _paymentService;

    public ClientsController(IBillService billService, IPaymentService paymentService)
    {
        _billService = billService;
        _paymentService = paymentService;
    }

    /// <summary>
    /// Retrieves all pending bills for a specific client.
    /// </summary>
    [HttpGet("{id}/pending-bills")]
    [ProducesResponseType(typeof(IEnumerable<BillDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPendingBills(int id)
    {
        var bills = await _billService.GetPendingBillsByClientIdAsync(id);
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
        var payments = await _paymentService.GetPaymentHistoryByClientIdAsync(id);
        return Ok(payments);
    }
}
