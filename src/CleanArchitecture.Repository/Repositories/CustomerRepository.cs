using System.Data;
using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using Dapper;

namespace CleanArchitecture.Repository.Repositories;

public class CustomerRepository(IUnitOfWork uow) : ICustomerRepository
{
    private IDbConnection Db => uow.Connection;

    public async Task<PagedResult<Customer>> GetRows(int page, int pageSize, string? search)
    {
        const string filter = @"
            @search IS NULL
            OR company_name ILIKE '%' || @search || '%'
            OR contact_name ILIKE '%' || @search || '%'
            OR city         ILIKE '%' || @search || '%'
            OR country      ILIKE '%' || @search || '%'";

        var countSql = $"SELECT COUNT(*) FROM customers WHERE {filter}";

        var pageSql = $@"
            SELECT
                    customer_id    AS CustomerID
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
            FROM
                    customers
            WHERE
                    {filter}
            ORDER BY
                    customer_id
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

        var args = new
        {
            search,
            offset = (page - 1) * pageSize,
            pageSize
        };

        var total = await Db.ExecuteScalarAsync<long>(countSql, args, uow.Transaction);
        var items = await Db.QueryAsync<Customer>(pageSql, args, uow.Transaction);
        return new PagedResult<Customer>(items, page, pageSize, total);
    }

    public async Task<Customer?> GetRow(string id)
    {
        const string sql = @"
            SELECT
                    customer_id    AS CustomerID
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
            FROM
                    customers
            WHERE
                    customer_id = @id";
        return await Db.QuerySingleOrDefaultAsync<Customer>(sql, new { id }, uow.Transaction);
    }

    public async Task<Customer> Insert(Customer customer)
    {
        const string sql = @"
            INSERT INTO customers
                    (customer_id, company_name, contact_name, contact_title, address, city, region, postal_code, country, phone, fax)
            VALUES
                    (
                        @CustomerID
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
                    )
            RETURNING
                    customer_id    AS CustomerID
                    ,company_name  AS CompanyName
                    ,contact_name  AS ContactName
                    ,contact_title AS ContactTitle
                    ,address       AS Address
                    ,city          AS City
                    ,region        AS Region
                    ,postal_code   AS PostalCode
                    ,country       AS Country
                    ,phone         AS Phone
                    ,fax           AS Fax";
        return await Db.QuerySingleAsync<Customer>(sql, customer, uow.Transaction);
    }

    public async Task<bool> Update(Customer customer)
    {
        const string sql = @"
            UPDATE customers
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
            WHERE
                    customer_id = @CustomerID";
        return await Db.ExecuteAsync(sql, customer, uow.Transaction) > 0;
    }

    public async Task<bool> Delete(string[] id)
    {
        const string sql = @"
            DELETE FROM customers
            WHERE
                    customer_id = ANY(@Ids)";
        return await Db.ExecuteAsync(sql, new { Ids = id }, uow.Transaction) > 0;
    }

    public async Task<int> CountExisting(string[] id)
    {
        const string sql = @"
            SELECT COUNT(*)
            FROM customers
            WHERE
                    customer_id = ANY(@Ids)";
        return await Db.ExecuteScalarAsync<int>(sql, new { Ids = id }, uow.Transaction);
    }
}
