using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;

namespace CleanArchitecture.Service.Services;

public class TerritoryService(ITerritoryRepository repo, IUnitOfWork uow) : ITerritoryService
{
    public Task<PagedResult<Territory>> GetRows(int page, int pageSize, string? search)
    {
        var keyword = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        return repo.GetRows(page, pageSize, keyword);
    }

    public Task<Territory?> GetRow(string id)
    {
        return repo.GetRow(id);
    }

    public Task<Territory> Insert(Territory territory)
    {
        return repo.Insert(territory);
    }

    public Task<bool> Update(string id, Territory territory)
    {
        territory.TerritoryID = id; // id dari route yang menang, bukan dari body
        return repo.Update(territory);
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
