using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Web.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController(ICustomerService service) : ControllerBase
{
    [HttpGet("GetRows")]
    public async Task<ActionResult<PagedResult<Customer>>> GetRows([FromQuery] PagedQuery query)
    {
        var result = await service.GetRows(query.Page, query.PageSize, query.Search);
        return Ok(result);
    }

    [HttpGet("GetRow/{id}")]
    public async Task<ActionResult<Customer>> GetRow(string id)
    {
        var customer = await service.GetRow(id);
        if (customer is null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [HttpPost("Insert")]
    public async Task<ActionResult<Customer>> Insert(SaveCustomerRequest request)
    {
        var customer = await service.Insert(ToEntity(request));
        return CreatedAtAction(nameof(GetRow), new { id = customer.CustomerID }, customer);
    }

    [HttpPut("Update/{id}")]
    public async Task<IActionResult> Update(string id, SaveCustomerRequest request)
    {
        var updated = await service.Update(id, ToEntity(request));
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete([FromQuery] string[] ids)
    {
        var deleted = await service.Delete(ids);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    private static Customer ToEntity(SaveCustomerRequest request)
    {
        return new Customer
        {
            CustomerID = request.CustomerID,
            CompanyName = request.CompanyName,
            ContactName = request.ContactName,
            ContactTitle = request.ContactTitle,
            Address = request.Address,
            City = request.City,
            Region = request.Region,
            PostalCode = request.PostalCode,
            Country = request.Country,
            Phone = request.Phone,
            Fax = request.Fax
        };
    }
}
