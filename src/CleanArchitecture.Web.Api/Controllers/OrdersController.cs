using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Web.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(IOrderService service) : ControllerBase
{
    [HttpGet("GetRows")]
    public async Task<ActionResult<PagedResult<Order>>> GetRows([FromQuery] PagedQuery query)
    {
        var result = await service.GetRows(query.Page, query.PageSize, query.Search);
        return Ok(result);
    }

    [HttpGet("GetRow/{id:int}")]
    public async Task<ActionResult<Order>> GetRow(int id)
    {
        var order = await service.GetRow(id);
        if (order is null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpPost("Insert")]
    public async Task<ActionResult<Order>> Insert(SaveOrderRequest request)
    {
        var order = await service.Insert(ToEntity(request));
        return CreatedAtAction(nameof(GetRow), new { id = order.OrderID }, order);
    }

    [HttpPut("Update/{id:int}")]
    public async Task<IActionResult> Update(int id, SaveOrderRequest request)
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

    private static Order ToEntity(SaveOrderRequest request)
    {
        return new Order
        {
            OrderID = request.OrderID,
            CustomerID = request.CustomerID,
            EmployeeID = request.EmployeeID,
            OrderDate = request.OrderDate,
            RequiredDate = request.RequiredDate,
            ShippedDate = request.ShippedDate,
            ShipVia = request.ShipVia,
            Freight = request.Freight,
            ShipName = request.ShipName,
            ShipAddress = request.ShipAddress,
            ShipCity = request.ShipCity,
            ShipRegion = request.ShipRegion,
            ShipPostalCode = request.ShipPostalCode,
            ShipCountry = request.ShipCountry
        };
    }
}
