using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;

namespace CleanArchitecture.Service.Services;

public class SupplierService(ISupplierRepository repo, IUnitOfWork uow) : ISupplierService
{
    public Task<PagedResult<Supplier>> GetRows(int page, int pageSize, string? search)
    {
        var keyword = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        return repo.GetRows(page, pageSize, keyword);
    }

    public Task<Supplier?> GetRow(int id)
    {
        return repo.GetRow(id);
    }

    public Task<Supplier> Insert(Supplier supplier)
    {
        return repo.Insert(supplier);
    }

    public Task<bool> Update(int id, Supplier supplier)
    {
        supplier.SupplierID = id; // id dari route yang menang, bukan dari body
        return repo.Update(supplier);
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
