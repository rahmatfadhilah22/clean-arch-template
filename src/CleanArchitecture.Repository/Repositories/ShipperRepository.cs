using System.Data;
using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using Dapper;

namespace CleanArchitecture.Repository.Repositories;

public class ShipperRepository(IUnitOfWork uow) : IShipperRepository
{
    private IDbConnection Db => uow.Connection;

    public async Task<PagedResult<Shipper>> GetRows(int page, int pageSize, string? search)
    {
        const string filter = @"
            @search IS NULL
            OR company_name ILIKE '%' || @search || '%'
            OR phone        ILIKE '%' || @search || '%'";

        var countSql = $"SELECT COUNT(*) FROM shippers WHERE {filter}";

        var pageSql = $@"
            SELECT
                    shipper_id    AS ShipperID
                    ,company_name AS CompanyName
                    ,phone        AS Phone
            FROM
                    shippers
            WHERE
                    {filter}
            ORDER BY
                    shipper_id
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

        var args = new
        {
            search,
            offset = (page - 1) * pageSize,
            pageSize
        };

        var total = await Db.ExecuteScalarAsync<long>(countSql, args, uow.Transaction);
        var items = await Db.QueryAsync<Shipper>(pageSql, args, uow.Transaction);
        return new PagedResult<Shipper>(items, page, pageSize, total);
    }

    public async Task<Shipper?> GetRow(int id)
    {
        const string sql = @"
            SELECT
                    shipper_id    AS ShipperID
                    ,company_name AS CompanyName
                    ,phone        AS Phone
            FROM
                    shippers
            WHERE
                    shipper_id = @id";
        return await Db.QuerySingleOrDefaultAsync<Shipper>(sql, new { id }, uow.Transaction);
    }

    // ponytail: id otomatis = MAX+1, bentrok kalau insert bersamaan; buat sequence kalau perlu.
    public async Task<Shipper> Insert(Shipper shipper)
    {
        const string sql = @"
            INSERT INTO shippers
                    (shipper_id, company_name, phone)
            VALUES
                    (
                        COALESCE(NULLIF(@ShipperID, 0), (SELECT COALESCE(MAX(shipper_id), 0) + 1 FROM shippers))
                        ,@CompanyName
                        ,@Phone
                    )
            RETURNING
                    shipper_id    AS ShipperID
                    ,company_name AS CompanyName
                    ,phone        AS Phone";
        return await Db.QuerySingleAsync<Shipper>(sql, shipper, uow.Transaction);
    }

    public async Task<bool> Update(Shipper shipper)
    {
        const string sql = @"
            UPDATE shippers
            SET
                    company_name = @CompanyName
                    ,phone = @Phone
            WHERE
                    shipper_id = @ShipperID";
        return await Db.ExecuteAsync(sql, shipper, uow.Transaction) > 0;
    }

    public async Task<bool> Delete(int[] id)
    {
        const string sql = @"
            DELETE FROM shippers
            WHERE
                    shipper_id = ANY(@Ids)";
        return await Db.ExecuteAsync(sql, new { Ids = id }, uow.Transaction) > 0;
    }

    public async Task<int> CountExisting(int[] id)
    {
        const string sql = @"
            SELECT COUNT(*)
            FROM shippers
            WHERE
                    shipper_id = ANY(@Ids)";
        return await Db.ExecuteScalarAsync<int>(sql, new { Ids = id }, uow.Transaction);
    }
}
