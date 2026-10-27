using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces;

public interface ICustomerDemographicRepository
{
    Task<PagedResult<CustomerDemographic>> GetRows(int page, int pageSize, string? search);
    Task<CustomerDemographic?> GetRow(string id);
    Task<CustomerDemographic> Insert(CustomerDemographic customerdemographic);
    Task<bool> Update(CustomerDemographic customerdemographic);
    Task<bool> Delete(string[] id);
    Task<int> CountExisting(string[] id);
}
