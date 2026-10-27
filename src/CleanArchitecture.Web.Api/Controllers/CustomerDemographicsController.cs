using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Web.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerDemographicsController(ICustomerDemographicService service) : ControllerBase
{
    [HttpGet("GetRows")]
    public async Task<ActionResult<PagedResult<CustomerDemographic>>> GetRows([FromQuery] PagedQuery query)
    {
        var result = await service.GetRows(query.Page, query.PageSize, query.Search);
        return Ok(result);
    }

    [HttpGet("GetRow/{id}")]
    public async Task<ActionResult<CustomerDemographic>> GetRow(string id)
    {
        var customerdemographic = await service.GetRow(id);
        if (customerdemographic is null)
        {
            return NotFound();
        }

        return Ok(customerdemographic);
    }

    [HttpPost("Insert")]
    public async Task<ActionResult<CustomerDemographic>> Insert(SaveCustomerDemographicRequest request)
    {
        var customerdemographic = await service.Insert(ToEntity(request));
        return CreatedAtAction(nameof(GetRow), new { id = customerdemographic.CustomerTypeID }, customerdemographic);
    }

    [HttpPut("Update/{id}")]
    public async Task<IActionResult> Update(string id, SaveCustomerDemographicRequest request)
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

    private static CustomerDemographic ToEntity(SaveCustomerDemographicRequest request)
    {
        return new CustomerDemographic
        {
            CustomerTypeID = request.CustomerTypeID,
            CustomerDesc = request.CustomerDesc
        };
    }
}
