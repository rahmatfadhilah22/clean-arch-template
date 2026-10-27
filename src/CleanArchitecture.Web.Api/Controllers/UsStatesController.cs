using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Web.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsStatesController(IUsStateService service) : ControllerBase
{
    [HttpGet("GetRows")]
    public async Task<ActionResult<PagedResult<UsState>>> GetRows([FromQuery] PagedQuery query)
    {
        var result = await service.GetRows(query.Page, query.PageSize, query.Search);
        return Ok(result);
    }

    [HttpGet("GetRow/{id:int}")]
    public async Task<ActionResult<UsState>> GetRow(int id)
    {
        var usstate = await service.GetRow(id);
        if (usstate is null)
        {
            return NotFound();
        }

        return Ok(usstate);
    }

    [HttpPost("Insert")]
    public async Task<ActionResult<UsState>> Insert(SaveUsStateRequest request)
    {
        var usstate = await service.Insert(ToEntity(request));
        return CreatedAtAction(nameof(GetRow), new { id = usstate.StateID }, usstate);
    }

    [HttpPut("Update/{id:int}")]
    public async Task<IActionResult> Update(int id, SaveUsStateRequest request)
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

    private static UsState ToEntity(SaveUsStateRequest request)
    {
        return new UsState
        {
            StateID = request.StateID,
            StateName = request.StateName,
            StateAbbr = request.StateAbbr,
            StateRegion = request.StateRegion
        };
    }
}
