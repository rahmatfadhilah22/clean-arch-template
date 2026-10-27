using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces;

public interface ICustomerRepository
{
    Task<PagedResult<Customer>> GetRows(int page, int pageSize, string? search);
    Task<Customer?> GetRow(string id);
    Task<Customer> Insert(Customer customer);
    Task<bool> Update(Customer customer);
    Task<bool> Delete(string[] id);
    Task<int> CountExisting(string[] id);
}
