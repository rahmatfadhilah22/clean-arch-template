using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Web.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService service) : ControllerBase
{
    [HttpGet("GetRows")]
    public async Task<ActionResult<PagedResult<Product>>> GetRows([FromQuery] PagedQuery query)
    {
        var result = await service.GetRows(query.Page, query.PageSize, query.Search);
        return Ok(result);
    }

    [HttpGet("GetRow/{id:int}")]
    public async Task<ActionResult<Product>> GetRow(int id)
    {
        var product = await service.GetRow(id);
        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost("Insert")]
    public async Task<ActionResult<Product>> Insert(SaveProductRequest request)
    {
        var product = await service.Insert(ToEntity(request));
        return CreatedAtAction(nameof(GetRow), new { id = product.ProductID }, product);
    }

    [HttpPut("Update/{id:int}")]
    public async Task<IActionResult> Update(int id, SaveProductRequest request)
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

    private static Product ToEntity(SaveProductRequest request)
    {
        return new Product
        {
            ProductID = request.ProductID,
            ProductName = request.ProductName,
            SupplierID = request.SupplierID,
            CategoryID = request.CategoryID,
            QuantityPerUnit = request.QuantityPerUnit,
            UnitPrice = request.UnitPrice,
            UnitsInStock = request.UnitsInStock,
            UnitsOnOrder = request.UnitsOnOrder,
            ReorderLevel = request.ReorderLevel,
            Discontinued = request.Discontinued
        };
    }
}
