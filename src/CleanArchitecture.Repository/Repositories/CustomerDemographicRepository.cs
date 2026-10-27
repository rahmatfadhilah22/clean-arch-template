using System.Data;
using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using Dapper;

namespace CleanArchitecture.Repository.Repositories;

public class CustomerDemographicRepository(IUnitOfWork uow) : ICustomerDemographicRepository
{
    private IDbConnection Db => uow.Connection;

    public async Task<PagedResult<CustomerDemographic>> GetRows(int page, int pageSize, string? search)
    {
        const string filter = @"
            @search IS NULL
            OR customer_type_id ILIKE '%' || @search || '%'
            OR customer_desc    ILIKE '%' || @search || '%'";

        var countSql = $"SELECT COUNT(*) FROM customer_demographics WHERE {filter}";

        var pageSql = $@"
            SELECT
                    customer_type_id  AS CustomerTypeID
                    ,customer_desc    AS CustomerDesc
            FROM
                    customer_demographics
            WHERE
                    {filter}
            ORDER BY
                    customer_type_id
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

        var args = new
        {
            search,
            offset = (page - 1) * pageSize,
            pageSize
        };

        var total = await Db.ExecuteScalarAsync<long>(countSql, args, uow.Transaction);
        var items = await Db.QueryAsync<CustomerDemographic>(pageSql, args, uow.Transaction);
        return new PagedResult<CustomerDemographic>(items, page, pageSize, total);
    }

    public async Task<CustomerDemographic?> GetRow(string id)
    {
        const string sql = @"
            SELECT
                    customer_type_id  AS CustomerTypeID
                    ,customer_desc    AS CustomerDesc
            FROM
                    customer_demographics
            WHERE
                    customer_type_id = @id";
        return await Db.QuerySingleOrDefaultAsync<CustomerDemographic>(sql, new { id }, uow.Transaction);
    }

    public async Task<CustomerDemographic> Insert(CustomerDemographic customerdemographic)
    {
        const string sql = @"
            INSERT INTO customer_demographics
                    (customer_type_id, customer_desc)
            VALUES
                    (
                        @CustomerTypeID
                        ,@CustomerDesc
                    )
            RETURNING
                    customer_type_id  AS CustomerTypeID
                    ,customer_desc    AS CustomerDesc";
        return await Db.QuerySingleAsync<CustomerDemographic>(sql, customerdemographic, uow.Transaction);
    }

    public async Task<bool> Update(CustomerDemographic customerdemographic)
    {
        const string sql = @"
            UPDATE customer_demographics
            SET
                    customer_desc = @CustomerDesc
            WHERE
                    customer_type_id = @CustomerTypeID";
        return await Db.ExecuteAsync(sql, customerdemographic, uow.Transaction) > 0;
    }

    public async Task<bool> Delete(string[] id)
    {
        const string sql = @"
            DELETE FROM customer_demographics
            WHERE
                    customer_type_id = ANY(@Ids)";
        return await Db.ExecuteAsync(sql, new { Ids = id }, uow.Transaction) > 0;
    }

    public async Task<int> CountExisting(string[] id)
    {
        const string sql = @"
            SELECT COUNT(*)
            FROM customer_demographics
            WHERE
                    customer_type_id = ANY(@Ids)";
        return await Db.ExecuteScalarAsync<int>(sql, new { Ids = id }, uow.Transaction);
    }
}
