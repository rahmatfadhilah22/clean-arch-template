using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;

namespace CleanArchitecture.Service.Services;

public class UsStateService(IUsStateRepository repo, IUnitOfWork uow) : IUsStateService
{
    public Task<PagedResult<UsState>> GetRows(int page, int pageSize, string? search)
    {
        var keyword = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        return repo.GetRows(page, pageSize, keyword);
    }

    public Task<UsState?> GetRow(int id)
    {
        return repo.GetRow(id);
    }

    public Task<UsState> Insert(UsState usstate)
    {
        return repo.Insert(usstate);
    }

    public Task<bool> Update(int id, UsState usstate)
    {
        usstate.StateID = id; // id dari route yang menang, bukan dari body
        return repo.Update(usstate);
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
