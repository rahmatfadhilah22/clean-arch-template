using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;

namespace CleanArchitecture.Service.Services;

public class RegionService(IRegionRepository repo, IUnitOfWork uow) : IRegionService
{
    public Task<PagedResult<Region>> GetRows(int page, int pageSize, string? search)
    {
        var keyword = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        return repo.GetRows(page, pageSize, keyword);
    }

    public Task<Region?> GetRow(int id)
    {
        return repo.GetRow(id);
    }

    public Task<Region> Insert(Region region)
    {
        return repo.Insert(region);
    }

    public Task<bool> Update(int id, Region region)
    {
        region.RegionID = id; // id dari route yang menang, bukan dari body
        return repo.Update(region);
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
