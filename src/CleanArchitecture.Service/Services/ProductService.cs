using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;

namespace CleanArchitecture.Service.Services;

public class ProductService(IProductRepository repo, IUnitOfWork uow) : IProductService
{
    public Task<PagedResult<Product>> GetRows(int page, int pageSize, string? search)
    {
        var keyword = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        return repo.GetRows(page, pageSize, keyword);
    }

    public Task<Product?> GetRow(int id)
    {
        return repo.GetRow(id);
    }

    public Task<Product> Insert(Product product)
    {
        return repo.Insert(product);
    }

    public Task<bool> Update(int id, Product product)
    {
        product.ProductID = id; // id dari route yang menang, bukan dari body
        return repo.Update(product);
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
