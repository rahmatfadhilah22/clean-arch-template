using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces;

public interface IEmployeeRepository
{
    Task<PagedResult<Employee>> GetRows(int page, int pageSize, string? search);
    Task<Employee?> GetRow(int id);
    Task<Employee> Insert(Employee employee);
    Task<bool> Update(Employee employee);
    Task<bool> Delete(int[] id);
    Task<int> CountExisting(int[] id);
}
