using Microsoft.AspNetCore.Mvc;
using BasicBilling.API.Application.DTOs;
using BasicBilling.API.Application.Interfaces.Services;

namespace BasicBilling.API.Controllers;

/// <summary>
/// Manages service bill operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BillsController : ControllerBase
{
    private readonly IBillService _billService;

    public BillsController(IBillService billService)
    {
        _billService = billService;
    }

    /// <summary>
    /// Creates a new service bill for administrative or testing purposes.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(BillDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateBill([FromBody] CreateBillDto dto)
    {
        var bill = await _billService.CreateBillAsync(dto);
        return CreatedAtAction(nameof(CreateBill), new { id = bill.Id }, bill);
    }
}
