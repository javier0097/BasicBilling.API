using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BasicBilling.API.Application.DTOs;
using BasicBilling.API.Application.Features.Bills.Commands;
using MediatR;

namespace BasicBilling.API.Controllers;

/// <summary>
/// Manages service bill operations.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BillsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BillsController(IMediator mediator)
    {
        _mediator = mediator;
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
        var bill = await _mediator.Send(new CreateBillCommand(dto));
        return CreatedAtAction(nameof(CreateBill), new { id = bill.Id }, bill);
    }
}
