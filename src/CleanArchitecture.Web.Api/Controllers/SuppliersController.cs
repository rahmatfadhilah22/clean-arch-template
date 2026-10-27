using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Web.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController(ISupplierService service) : ControllerBase
{
    [HttpGet("GetRows")]
    public async Task<ActionResult<PagedResult<Supplier>>> GetRows([FromQuery] PagedQuery query)
    {
        var result = await service.GetRows(query.Page, query.PageSize, query.Search);
        return Ok(result);
    }

    [HttpGet("GetRow/{id:int}")]
    public async Task<ActionResult<Supplier>> GetRow(int id)
    {
        var supplier = await service.GetRow(id);
        if (supplier is null)
        {
            return NotFound();
        }

        return Ok(supplier);
    }

    [HttpPost("Insert")]
    public async Task<ActionResult<Supplier>> Insert(SaveSupplierRequest request)
    {
        var supplier = await service.Insert(ToEntity(request));
        return CreatedAtAction(nameof(GetRow), new { id = supplier.SupplierID }, supplier);
    }

    [HttpPut("Update/{id:int}")]
    public async Task<IActionResult> Update(int id, SaveSupplierRequest request)
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

    private static Supplier ToEntity(SaveSupplierRequest request)
    {
        return new Supplier
        {
            SupplierID = request.SupplierID,
            CompanyName = request.CompanyName,
            ContactName = request.ContactName,
            ContactTitle = request.ContactTitle,
            Address = request.Address,
            City = request.City,
            Region = request.Region,
            PostalCode = request.PostalCode,
            Country = request.Country,
            Phone = request.Phone,
            Fax = request.Fax,
            Homepage = request.Homepage
        };
    }
}
