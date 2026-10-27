using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Web.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TerritoriesController(ITerritoryService service) : ControllerBase
{
    [HttpGet("GetRows")]
    public async Task<ActionResult<PagedResult<Territory>>> GetRows([FromQuery] PagedQuery query)
    {
        var result = await service.GetRows(query.Page, query.PageSize, query.Search);
        return Ok(result);
    }

    [HttpGet("GetRow/{id}")]
    public async Task<ActionResult<Territory>> GetRow(string id)
    {
        var territory = await service.GetRow(id);
        if (territory is null)
        {
            return NotFound();
        }

        return Ok(territory);
    }

    [HttpPost("Insert")]
    public async Task<ActionResult<Territory>> Insert(SaveTerritoryRequest request)
    {
        var territory = await service.Insert(ToEntity(request));
        return CreatedAtAction(nameof(GetRow), new { id = territory.TerritoryID }, territory);
    }

    [HttpPut("Update/{id}")]
    public async Task<IActionResult> Update(string id, SaveTerritoryRequest request)
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

    private static Territory ToEntity(SaveTerritoryRequest request)
    {
        return new Territory
        {
            TerritoryID = request.TerritoryID,
            TerritoryDescription = request.TerritoryDescription,
            RegionID = request.RegionID
        };
    }
}
