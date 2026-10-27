using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;

namespace CleanArchitecture.Service.Services;

public class ShipperService(IShipperRepository repo, IUnitOfWork uow) : IShipperService
{
    public Task<PagedResult<Shipper>> GetRows(int page, int pageSize, string? search)
    {
        var keyword = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        return repo.GetRows(page, pageSize, keyword);
    }

    public Task<Shipper?> GetRow(int id)
    {
        return repo.GetRow(id);
    }

    public Task<Shipper> Insert(Shipper shipper)
    {
        return repo.Insert(shipper);
    }

    public Task<bool> Update(int id, Shipper shipper)
    {
        shipper.ShipperID = id; // id dari route yang menang, bukan dari body
        return repo.Update(shipper);
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
