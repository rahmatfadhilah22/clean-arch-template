using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Service.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Service;

public static class DependencyInjection
{
    public static IServiceCollection AddService(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        return services;
    }
}
