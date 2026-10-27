using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Web.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController(IEmployeeService service) : ControllerBase
{
    [HttpGet("GetRows")]
    public async Task<ActionResult<PagedResult<Employee>>> GetRows([FromQuery] PagedQuery query)
    {
        var result = await service.GetRows(query.Page, query.PageSize, query.Search);
        return Ok(result);
    }

    [HttpGet("GetRow/{id:int}")]
    public async Task<ActionResult<Employee>> GetRow(int id)
    {
        var employee = await service.GetRow(id);
        if (employee is null)
        {
            return NotFound();
        }

        return Ok(employee);
    }

    [HttpPost("Insert")]
    public async Task<ActionResult<Employee>> Insert(SaveEmployeeRequest request)
    {
        var employee = await service.Insert(ToEntity(request));
        return CreatedAtAction(nameof(GetRow), new { id = employee.EmployeeID }, employee);
    }

    [HttpPut("Update/{id:int}")]
    public async Task<IActionResult> Update(int id, SaveEmployeeRequest request)
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

    private static Employee ToEntity(SaveEmployeeRequest request)
    {
        return new Employee
        {
            EmployeeID = request.EmployeeID,
            LastName = request.LastName,
            FirstName = request.FirstName,
            Title = request.Title,
            TitleOfCourtesy = request.TitleOfCourtesy,
            BirthDate = request.BirthDate,
            HireDate = request.HireDate,
            Address = request.Address,
            City = request.City,
            Region = request.Region,
            PostalCode = request.PostalCode,
            Country = request.Country,
            HomePhone = request.HomePhone,
            Extension = request.Extension,
            Photo = request.Photo,
            Notes = request.Notes,
            ReportsTo = request.ReportsTo,
            PhotoPath = request.PhotoPath
        };
    }
}
