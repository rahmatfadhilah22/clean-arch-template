using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;

namespace CleanArchitecture.Service.Services;

public class EmployeeService(IEmployeeRepository repo, IUnitOfWork uow) : IEmployeeService
{
    public Task<PagedResult<Employee>> GetRows(int page, int pageSize, string? search)
    {
        var keyword = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        return repo.GetRows(page, pageSize, keyword);
    }

    public Task<Employee?> GetRow(int id)
    {
        return repo.GetRow(id);
    }

    public Task<Employee> Insert(Employee employee)
    {
        return repo.Insert(employee);
    }

    public Task<bool> Update(int id, Employee employee)
    {
        employee.EmployeeID = id; // id dari route yang menang, bukan dari body
        return repo.Update(employee);
    }

    public async Task<bool> Delete(int[] id)
    {
        await uow.BeginAsync();
        try
        {
            int found = await repo.CountExisting(id);
            if (found != id.Length)
            {
                await uow.RollbackAsync();
                return false;
            }

            bool deleted = await repo.Delete(id);
            await uow.CommitAsync();
            return deleted;
        }
        catch
        {
            await uow.RollbackAsync();
            throw;
        }
    }
}
