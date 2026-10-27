using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces;

public interface IEmployeeService
{
    Task<PagedResult<Employee>> GetRows(int page, int pageSize, string? search);
    Task<Employee?> GetRow(int id);
    Task<Employee> Insert(Employee employee);
    Task<bool> Update(int id, Employee employee);
    Task<bool> Delete(int[] id);
}
