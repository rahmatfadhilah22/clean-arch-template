using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;

namespace CleanArchitecture.Service.Services;

public class CustomerService(ICustomerRepository repo, IUnitOfWork uow) : ICustomerService
{
    public Task<PagedResult<Customer>> GetRows(int page, int pageSize, string? search)
    {
        var keyword = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        return repo.GetRows(page, pageSize, keyword);
    }

    public Task<Customer?> GetRow(string id)
    {
        return repo.GetRow(id);
    }

    public Task<Customer> Insert(Customer customer)
    {
        return repo.Insert(customer);
    }

    public Task<bool> Update(string id, Customer customer)
    {
        customer.CustomerID = id; // id dari route yang menang, bukan dari body
        return repo.Update(customer);
    }

    public async Task<bool> Delete(string[] id)
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
