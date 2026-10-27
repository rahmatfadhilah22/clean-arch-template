using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Web.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryService service) : ControllerBase
{
    [HttpGet("GetRows")]
    public async Task<ActionResult<PagedResult<Category>>> GetRows([FromQuery] PagedQuery query)
    {
        var result = await service.GetRows(query.Page, query.PageSize, query.Search);
        return Ok(result);
    }

    [HttpGet("GetRow/{id:int}")]
    public async Task<ActionResult<Category>> GetRow(int id)
    {
        var category = await service.GetRow(id);
        if (category is null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    [HttpPost("Insert")]
    public async Task<ActionResult<Category>> Insert(SaveCategoryRequest request)
    {
        var category = await service.Insert(ToEntity(request));
        return CreatedAtAction(nameof(GetRow), new { id = category.CategoryID }, category);
    }

    [HttpPut("Update/{id:int}")]
    public async Task<IActionResult> Update(int id, SaveCategoryRequest request)
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
    private static Category ToEntity(SaveCategoryRequest request)
    {
        return new Category
        {
            CategoryID = request.CategoryId,
            CategoryName = request.CategoryName,
            Description = request.Description,
            Picture = request.Picture
        };
    }
}
