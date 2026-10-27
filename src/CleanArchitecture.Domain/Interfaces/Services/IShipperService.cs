using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces;

public interface IShipperService
{
    Task<PagedResult<Shipper>> GetRows(int page, int pageSize, string? search);
    Task<Shipper?> GetRow(int id);
    Task<Shipper> Insert(Shipper shipper);
    Task<bool> Update(int id, Shipper shipper);
    Task<bool> Delete(int[] id);
}
