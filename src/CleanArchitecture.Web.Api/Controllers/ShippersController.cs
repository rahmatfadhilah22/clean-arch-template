using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Web.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShippersController(IShipperService service) : ControllerBase
{
    [HttpGet("GetRows")]
    public async Task<ActionResult<PagedResult<Shipper>>> GetRows([FromQuery] PagedQuery query)
    {
        var result = await service.GetRows(query.Page, query.PageSize, query.Search);
        return Ok(result);
    }

    [HttpGet("GetRow/{id:int}")]
    public async Task<ActionResult<Shipper>> GetRow(int id)
    {
        var shipper = await service.GetRow(id);
        if (shipper is null)
        {
            return NotFound();
        }

        return Ok(shipper);
    }

    [HttpPost("Insert")]
    public async Task<ActionResult<Shipper>> Insert(SaveShipperRequest request)
    {
        var shipper = await service.Insert(ToEntity(request));
        return CreatedAtAction(nameof(GetRow), new { id = shipper.ShipperID }, shipper);
    }

    [HttpPut("Update/{id:int}")]
    public async Task<IActionResult> Update(int id, SaveShipperRequest request)
    {
        var updated = await service.Update(id, ToEntity(request));
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete([FromQuery] int[] ids)
    {
        var deleted = await service.Delete(ids);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    private static Shipper ToEntity(SaveShipperRequest request)
    {
        return new Shipper
        {
            ShipperID = request.ShipperID,
            CompanyName = request.CompanyName,
            Phone = request.Phone
        };
    }
}
