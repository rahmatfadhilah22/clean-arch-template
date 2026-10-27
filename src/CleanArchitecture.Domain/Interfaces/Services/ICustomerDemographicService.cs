using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces;

public interface ICustomerDemographicService
{
    Task<PagedResult<CustomerDemographic>> GetRows(int page, int pageSize, string? search);
    Task<CustomerDemographic?> GetRow(string id);
    Task<CustomerDemographic> Insert(CustomerDemographic customerdemographic);
    Task<bool> Update(string id, CustomerDemographic customerdemographic);
    Task<bool> Delete(string[] id);
}
