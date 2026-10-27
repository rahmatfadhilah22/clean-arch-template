using System.Data;
using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using Dapper;

namespace CleanArchitecture.Repository.Repositories;

public class SupplierRepository(IUnitOfWork uow) : ISupplierRepository
{
    private IDbConnection Db => uow.Connection;

    public async Task<PagedResult<Supplier>> GetRows(int page, int pageSize, string? search)
    {
        const string filter = @"
            @search IS NULL
            OR company_name ILIKE '%' || @search || '%'
            OR contact_name ILIKE '%' || @search || '%'
            OR city         ILIKE '%' || @search || '%'
            OR country      ILIKE '%' || @search || '%'";

        var countSql = $"SELECT COUNT(*) FROM suppliers WHERE {filter}";

        var pageSql = $@"
            SELECT
                    supplier_id    AS SupplierID
                    ,company_name  AS CompanyName
                    ,contact_name  AS ContactName
                    ,contact_title AS ContactTitle
                    ,address       AS Address
                    ,city          AS City
                    ,region        AS Region
                    ,postal_code   AS PostalCode
                    ,country       AS Country
                    ,phone         AS Phone
                    ,fax           AS Fax
                    ,homepage      AS Homepage
            FROM
                    suppliers
            WHERE
                    {filter}
            ORDER BY
                    supplier_id
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

        var args = new
        {
            search,
            offset = (page - 1) * pageSize,
            pageSize
        };

        var total = await Db.ExecuteScalarAsync<long>(countSql, args, uow.Transaction);
        var items = await Db.QueryAsync<Supplier>(pageSql, args, uow.Transaction);
        return new PagedResult<Supplier>(items, page, pageSize, total);
    }

    public async Task<Supplier?> GetRow(int id)
    {
        const string sql = @"
            SELECT
                    supplier_id    AS SupplierID
                    ,company_name  AS CompanyName
                    ,contact_name  AS ContactName
                    ,contact_title AS ContactTitle
                    ,address       AS Address
                    ,city          AS City
                    ,region        AS Region
                    ,postal_code   AS PostalCode
                    ,country       AS Country
                    ,phone         AS Phone
                    ,fax           AS Fax
                    ,homepage      AS Homepage
            FROM
                    suppliers
            WHERE
                    supplier_id = @id";
        return await Db.QuerySingleOrDefaultAsync<Supplier>(sql, new { id }, uow.Transaction);
    }

    // ponytail: id otomatis = MAX+1, bentrok kalau insert bersamaan; buat sequence kalau perlu.
    public async Task<Supplier> Insert(Supplier supplier)
    {
        const string sql = @"
            INSERT INTO suppliers
                    (supplier_id, company_name, contact_name, contact_title, address, city, region, postal_code, country, phone, fax, homepage)
            VALUES
                    (
                        COALESCE(NULLIF(@SupplierID, 0), (SELECT COALESCE(MAX(supplier_id), 0) + 1 FROM suppliers))
                        ,@CompanyName
                        ,@ContactName
                        ,@ContactTitle
                        ,@Address
                        ,@City
                        ,@Region
                        ,@PostalCode
                        ,@Country
                        ,@Phone
                        ,@Fax
                        ,@Homepage
                    )
            RETURNING
                    supplier_id    AS SupplierID
                    ,company_name  AS CompanyName
                    ,contact_name  AS ContactName
                    ,contact_title AS ContactTitle
                    ,address       AS Address
                    ,city          AS City
                    ,region        AS Region
                    ,postal_code   AS PostalCode
                    ,country       AS Country
                    ,phone         AS Phone
                    ,fax           AS Fax
                    ,homepage      AS Homepage";
        return await Db.QuerySingleAsync<Supplier>(sql, supplier, uow.Transaction);
    }

    public async Task<bool> Update(Supplier supplier)
    {
        const string sql = @"
            UPDATE suppliers
            SET
                    company_name = @CompanyName
                    ,contact_name = @ContactName
                    ,contact_title = @ContactTitle
                    ,address = @Address
                    ,city = @City
                    ,region = @Region
                    ,postal_code = @PostalCode
                    ,country = @Country
                    ,phone = @Phone
                    ,fax = @Fax
                    ,homepage = @Homepage
            WHERE
                    supplier_id = @SupplierID";
        return await Db.ExecuteAsync(sql, supplier, uow.Transaction) > 0;
    }

    public async Task<bool> Delete(int[] id)
    {
        const string sql = @"
            DELETE FROM suppliers
            WHERE
                    supplier_id = ANY(@Ids)";
        return await Db.ExecuteAsync(sql, new { Ids = id }, uow.Transaction) > 0;
    }

    public async Task<int> CountExisting(int[] id)
    {
        const string sql = @"
            SELECT COUNT(*)
            FROM suppliers
            WHERE
                    supplier_id = ANY(@Ids)";
        return await Db.ExecuteScalarAsync<int>(sql, new { Ids = id }, uow.Transaction);
    }
}
