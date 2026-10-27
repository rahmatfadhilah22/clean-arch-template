using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;

namespace CleanArchitecture.Service.Services;

public class OrderService(IOrderRepository repo, IUnitOfWork uow) : IOrderService
{
    public Task<PagedResult<Order>> GetRows(int page, int pageSize, string? search)
    {
        var keyword = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        return repo.GetRows(page, pageSize, keyword);
    }

    public Task<Order?> GetRow(int id)
    {
        return repo.GetRow(id);
    }

    public Task<Order> Insert(Order order)
    {
        return repo.Insert(order);
    }

    public Task<bool> Update(int id, Order order)
    {
        order.OrderID = id; // id dari route yang menang, bukan dari body
        return repo.Update(order);
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
