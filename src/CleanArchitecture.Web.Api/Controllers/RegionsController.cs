using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Web.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegionsController(IRegionService service) : ControllerBase
{
    [HttpGet("GetRows")]
    public async Task<ActionResult<PagedResult<Region>>> GetRows([FromQuery] PagedQuery query)
    {
        var result = await service.GetRows(query.Page, query.PageSize, query.Search);
        return Ok(result);
    }

    [HttpGet("GetRow/{id:int}")]
    public async Task<ActionResult<Region>> GetRow(int id)
    {
        var region = await service.GetRow(id);
        if (region is null)
        {
            return NotFound();
        }

        return Ok(region);
    }

    [HttpPost("Insert")]
    public async Task<ActionResult<Region>> Insert(SaveRegionRequest request)
    {
        var region = await service.Insert(ToEntity(request));
        return CreatedAtAction(nameof(GetRow), new { id = region.RegionID }, region);
    }

    [HttpPut("Update/{id:int}")]
    public async Task<IActionResult> Update(int id, SaveRegionRequest request)
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

    private static Region ToEntity(SaveRegionRequest request)
    {
        return new Region
        {
            RegionID = request.RegionID,
            RegionDescription = request.RegionDescription
        };
    }
}
