using System.Data;
using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using Dapper;

namespace CleanArchitecture.Repository.Repositories;

public class EmployeeRepository(IUnitOfWork uow) : IEmployeeRepository
{
    private IDbConnection Db => uow.Connection;

    public async Task<PagedResult<Employee>> GetRows(int page, int pageSize, string? search)
    {
        const string filter = @"
            @search IS NULL
            OR first_name ILIKE '%' || @search || '%'
            OR last_name  ILIKE '%' || @search || '%'
            OR title      ILIKE '%' || @search || '%'
            OR city       ILIKE '%' || @search || '%'";

        var countSql = $"SELECT COUNT(*) FROM employees WHERE {filter}";

        var pageSql = $@"
            SELECT
                    employee_id        AS EmployeeID
                    ,last_name         AS LastName
                    ,first_name        AS FirstName
                    ,title             AS Title
                    ,title_of_courtesy AS TitleOfCourtesy
                    ,birth_date        AS BirthDate
                    ,hire_date         AS HireDate
                    ,address           AS Address
                    ,city              AS City
                    ,region            AS Region
                    ,postal_code       AS PostalCode
                    ,country           AS Country
                    ,home_phone        AS HomePhone
                    ,extension         AS Extension
                    ,photo             AS Photo
                    ,notes             AS Notes
                    ,reports_to        AS ReportsTo
                    ,photo_path        AS PhotoPath
            FROM
                    employees
            WHERE
                    {filter}
            ORDER BY
                    employee_id
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

        var args = new
        {
            search,
            offset = (page - 1) * pageSize,
            pageSize
        };

        var total = await Db.ExecuteScalarAsync<long>(countSql, args, uow.Transaction);
        var items = await Db.QueryAsync<Employee>(pageSql, args, uow.Transaction);
        return new PagedResult<Employee>(items, page, pageSize, total);
    }

    public async Task<Employee?> GetRow(int id)
    {
        const string sql = @"
            SELECT
                    employee_id        AS EmployeeID
                    ,last_name         AS LastName
                    ,first_name        AS FirstName
                    ,title             AS Title
                    ,title_of_courtesy AS TitleOfCourtesy
                    ,birth_date        AS BirthDate
                    ,hire_date         AS HireDate
                    ,address           AS Address
                    ,city              AS City
                    ,region            AS Region
                    ,postal_code       AS PostalCode
                    ,country           AS Country
                    ,home_phone        AS HomePhone
                    ,extension         AS Extension
                    ,photo             AS Photo
                    ,notes             AS Notes
                    ,reports_to        AS ReportsTo
                    ,photo_path        AS PhotoPath
            FROM
                    employees
            WHERE
                    employee_id = @id";
        return await Db.QuerySingleOrDefaultAsync<Employee>(sql, new { id }, uow.Transaction);
    }

    // ponytail: id otomatis = MAX+1, bentrok kalau insert bersamaan; buat sequence kalau perlu.
    public async Task<Employee> Insert(Employee employee)
    {
        const string sql = @"
            INSERT INTO employees
                    (employee_id, last_name, first_name, title, title_of_courtesy, birth_date, hire_date, address, city, region, postal_code, country, home_phone, extension, photo, notes, reports_to, photo_path)
            VALUES
                    (
                        COALESCE(NULLIF(@EmployeeID, 0), (SELECT COALESCE(MAX(employee_id), 0) + 1 FROM employees))
                        ,@LastName
                        ,@FirstName
                        ,@Title
                        ,@TitleOfCourtesy
                        ,@BirthDate
                        ,@HireDate
                        ,@Address
                        ,@City
                        ,@Region
                        ,@PostalCode
                        ,@Country
                        ,@HomePhone
                        ,@Extension
                        ,@Photo
                        ,@Notes
                        ,@ReportsTo
                        ,@PhotoPath
                    )
            RETURNING
                    employee_id        AS EmployeeID
                    ,last_name         AS LastName
                    ,first_name        AS FirstName
                    ,title             AS Title
                    ,title_of_courtesy AS TitleOfCourtesy
                    ,birth_date        AS BirthDate
                    ,hire_date         AS HireDate
                    ,address           AS Address
                    ,city              AS City
                    ,region            AS Region
                    ,postal_code       AS PostalCode
                    ,country           AS Country
                    ,home_phone        AS HomePhone
                    ,extension         AS Extension
                    ,photo             AS Photo
                    ,notes             AS Notes
                    ,reports_to        AS ReportsTo
                    ,photo_path        AS PhotoPath";
        return await Db.QuerySingleAsync<Employee>(sql, employee, uow.Transaction);
    }

    public async Task<bool> Update(Employee employee)
    {
        const string sql = @"
            UPDATE employees
            SET
                    last_name = @LastName
                    ,first_name = @FirstName
                    ,title = @Title
                    ,title_of_courtesy = @TitleOfCourtesy
                    ,birth_date = @BirthDate
                    ,hire_date = @HireDate
                    ,address = @Address
                    ,city = @City
                    ,region = @Region
                    ,postal_code = @PostalCode
                    ,country = @Country
                    ,home_phone = @HomePhone
                    ,extension = @Extension
                    ,photo = @Photo
                    ,notes = @Notes
                    ,reports_to = @ReportsTo
                    ,photo_path = @PhotoPath
            WHERE
                    employee_id = @EmployeeID";
        return await Db.ExecuteAsync(sql, employee, uow.Transaction) > 0;
    }

    public async Task<bool> Delete(int[] id)
    {
        const string sql = @"
            DELETE FROM employees
            WHERE
                    employee_id = ANY(@Ids)";
        return await Db.ExecuteAsync(sql, new { Ids = id }, uow.Transaction) > 0;
    }

    public async Task<int> CountExisting(int[] id)
    {
        const string sql = @"
            SELECT COUNT(*)
            FROM employees
            WHERE
                    employee_id = ANY(@Ids)";
        return await Db.ExecuteScalarAsync<int>(sql, new { Ids = id }, uow.Transaction);
    }
}
