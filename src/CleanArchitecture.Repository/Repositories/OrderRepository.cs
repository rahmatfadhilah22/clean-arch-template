using System.Data;
using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using Dapper;

namespace CleanArchitecture.Repository.Repositories;

public class OrderRepository(IUnitOfWork uow) : IOrderRepository
{
    private IDbConnection Db => uow.Connection;

    public async Task<PagedResult<Order>> GetRows(int page, int pageSize, string? search)
    {
        const string filter = @"
            @search IS NULL
            OR o.customer_id  ILIKE '%' || @search || '%'
            OR c.company_name ILIKE '%' || @search || '%'
            OR e.first_name   ILIKE '%' || @search || '%'
            OR e.last_name    ILIKE '%' || @search || '%'
            OR s.company_name ILIKE '%' || @search || '%'
            OR o.ship_name    ILIKE '%' || @search || '%'
            OR o.ship_city    ILIKE '%' || @search || '%'
            OR o.ship_country ILIKE '%' || @search || '%'";

        const string joins = @"
            FROM orders o
            LEFT JOIN customers c ON c.customer_id = o.customer_id
            LEFT JOIN employees e ON e.employee_id = o.employee_id
            LEFT JOIN shippers  s ON s.shipper_id  = o.ship_via";

        var countSql = $"SELECT COUNT(*) {joins} WHERE {filter}";

        var pageSql = $@"
            SELECT
                    o.order_id          AS OrderID
                    ,o.customer_id      AS CustomerID
                    ,o.employee_id      AS EmployeeID
                    ,o.order_date       AS OrderDate
                    ,o.required_date    AS RequiredDate
                    ,o.shipped_date     AS ShippedDate
                    ,o.ship_via         AS ShipVia
                    ,o.freight          AS Freight
                    ,o.ship_name        AS ShipName
                    ,o.ship_address     AS ShipAddress
                    ,o.ship_city        AS ShipCity
                    ,o.ship_region      AS ShipRegion
                    ,o.ship_postal_code AS ShipPostalCode
                    ,o.ship_country     AS ShipCountry
                    ,c.company_name     AS CustomerName
                    ,CONCAT_WS(' ', e.first_name, e.last_name) AS EmployeeName
                    ,s.company_name     AS ShipperName
            {joins}
            WHERE
                    {filter}
            ORDER BY
                    o.order_id
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

        var args = new
        {
            search,
            offset = (page - 1) * pageSize,
            pageSize
        };

        var total = await Db.ExecuteScalarAsync<long>(countSql, args, uow.Transaction);
        var items = await Db.QueryAsync<Order>(pageSql, args, uow.Transaction);
        return new PagedResult<Order>(items, page, pageSize, total);
    }

    public async Task<Order?> GetRow(int id)
    {
        const string sql = @"
            SELECT
                    order_id          AS OrderID
                    ,customer_id      AS CustomerID
                    ,employee_id      AS EmployeeID
                    ,order_date       AS OrderDate
                    ,required_date    AS RequiredDate
                    ,shipped_date     AS ShippedDate
                    ,ship_via         AS ShipVia
                    ,freight          AS Freight
                    ,ship_name        AS ShipName
                    ,ship_address     AS ShipAddress
                    ,ship_city        AS ShipCity
                    ,ship_region      AS ShipRegion
                    ,ship_postal_code AS ShipPostalCode
                    ,ship_country     AS ShipCountry
            FROM
                    orders
            WHERE
                    order_id = @id";
        return await Db.QuerySingleOrDefaultAsync<Order>(sql, new { id }, uow.Transaction);
    }

    // ponytail: id otomatis = MAX+1, bentrok kalau insert bersamaan; buat sequence kalau perlu.
    public async Task<Order> Insert(Order order)
    {
        const string sql = @"
            INSERT INTO orders
                    (order_id, customer_id, employee_id, order_date, required_date, shipped_date, ship_via, freight, ship_name, ship_address, ship_city, ship_region, ship_postal_code, ship_country)
            VALUES
                    (
                        COALESCE(NULLIF(@OrderID, 0), (SELECT COALESCE(MAX(order_id), 0) + 1 FROM orders))
                        ,@CustomerID
                        ,@EmployeeID
                        ,@OrderDate
                        ,@RequiredDate
                        ,@ShippedDate
                        ,@ShipVia
                        ,@Freight
                        ,@ShipName
                        ,@ShipAddress
                        ,@ShipCity
                        ,@ShipRegion
                        ,@ShipPostalCode
                        ,@ShipCountry
                    )
            RETURNING
                    order_id          AS OrderID
                    ,customer_id      AS CustomerID
                    ,employee_id      AS EmployeeID
                    ,order_date       AS OrderDate
                    ,required_date    AS RequiredDate
                    ,shipped_date     AS ShippedDate
                    ,ship_via         AS ShipVia
                    ,freight          AS Freight
                    ,ship_name        AS ShipName
                    ,ship_address     AS ShipAddress
                    ,ship_city        AS ShipCity
                    ,ship_region      AS ShipRegion
                    ,ship_postal_code AS ShipPostalCode
                    ,ship_country     AS ShipCountry";
        return await Db.QuerySingleAsync<Order>(sql, order, uow.Transaction);
    }

    public async Task<bool> Update(Order order)
    {
        const string sql = @"
            UPDATE orders
            SET
                    customer_id = @CustomerID
                    ,employee_id = @EmployeeID
                    ,order_date = @OrderDate
                    ,required_date = @RequiredDate
                    ,shipped_date = @ShippedDate
                    ,ship_via = @ShipVia
                    ,freight = @Freight
                    ,ship_name = @ShipName
                    ,ship_address = @ShipAddress
                    ,ship_city = @ShipCity
                    ,ship_region = @ShipRegion
                    ,ship_postal_code = @ShipPostalCode
                    ,ship_country = @ShipCountry
            WHERE
                    order_id = @OrderID";
        return await Db.ExecuteAsync(sql, order, uow.Transaction) > 0;
    }

    public async Task<bool> Delete(int[] id)
    {
        const string sql = @"
            DELETE FROM orders
            WHERE
                    order_id = ANY(@Ids)";
        return await Db.ExecuteAsync(sql, new { Ids = id }, uow.Transaction) > 0;
    }

    public async Task<int> CountExisting(int[] id)
    {
        const string sql = @"
            SELECT COUNT(*)
            FROM orders
            WHERE
                    order_id = ANY(@Ids)";
        return await Db.ExecuteScalarAsync<int>(sql, new { Ids = id }, uow.Transaction);
    }
}
