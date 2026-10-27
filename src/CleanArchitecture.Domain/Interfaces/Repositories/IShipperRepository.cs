using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces;

public interface IShipperRepository
{
    Task<PagedResult<Shipper>> GetRows(int page, int pageSize, string? search);
    Task<Shipper?> GetRow(int id);
    Task<Shipper> Insert(Shipper shipper);
    Task<bool> Update(Shipper shipper);
    Task<bool> Delete(int[] id);
    Task<int> CountExisting(int[] id);
}
