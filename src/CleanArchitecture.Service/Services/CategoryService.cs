using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;

namespace CleanArchitecture.Service.Services;

public class CategoryService(ICategoryRepository repo, IUnitOfWork uow) : ICategoryService
{
    public Task<PagedResult<Category>> GetRows(int page, int pageSize, string? search)
    {
        var keyword = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        return repo.GetRows(page, pageSize, keyword);
    }

    public Task<Category?> GetRow(int id)
    {
        return repo.GetRow(id);
    }

    public Task<Category> Insert(Category category)
    {
        return repo.Insert(category);
    }

    public Task<bool> Update(int id, Category category)
    {
        category.CategoryID = id; // id dari route yang menang, bukan dari body
        return repo.Update(category);
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
