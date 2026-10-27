using System.Data;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Repository.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Repository;

public static class DependencyInjection
{
    public static IServiceCollection AddRepository(this IServiceCollection services, string connectionString)
    {
        // Satu koneksi scoped dipakai semua repo — syarat transaksi antar repo.
        services.AddScoped<IUnitOfWork>(sp => new SqlUnitOfWork(connectionString));
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICustomerDemographicRepository, CustomerDemographicRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IRegionRepository, RegionRepository>();
        services.AddScoped<IShipperRepository, ShipperRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<ITerritoryRepository, TerritoryRepository>();
        services.AddScoped<IUsStateRepository, UsStateRepository>();
        return services;
    }
}
