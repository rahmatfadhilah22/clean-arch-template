using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;

namespace CleanArchitecture.Service.Services;

public class CustomerDemographicService(ICustomerDemographicRepository repo, IUnitOfWork uow) : ICustomerDemographicService
{
    public Task<PagedResult<CustomerDemographic>> GetRows(int page, int pageSize, string? search)
    {
        var keyword = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        return repo.GetRows(page, pageSize, keyword);
    }

    public Task<CustomerDemographic?> GetRow(string id)
    {
        return repo.GetRow(id);
    }

    public Task<CustomerDemographic> Insert(CustomerDemographic customerdemographic)
    {
        return repo.Insert(customerdemographic);
    }

    public Task<bool> Update(string id, CustomerDemographic customerdemographic)
    {
        customerdemographic.CustomerTypeID = id; // id dari route yang menang, bukan dari body
        return repo.Update(customerdemographic);
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
