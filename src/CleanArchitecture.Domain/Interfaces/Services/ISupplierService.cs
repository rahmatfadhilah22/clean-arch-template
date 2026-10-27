using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces;

public interface ISupplierService
{
    Task<PagedResult<Supplier>> GetRows(int page, int pageSize, string? search);
    Task<Supplier?> GetRow(int id);
    Task<Supplier> Insert(Supplier supplier);
    Task<bool> Update(int id, Supplier supplier);
    Task<bool> Delete(int[] id);
}
