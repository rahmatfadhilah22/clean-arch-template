using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Service.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Service;

public static class DependencyInjection
{
    public static IServiceCollection AddService(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICustomerDemographicService, CustomerDemographicService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IRegionService, RegionService>();
        services.AddScoped<IShipperService, ShipperService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<ITerritoryService, TerritoryService>();
        services.AddScoped<IUsStateService, UsStateService>();
        return services;
    }
}
