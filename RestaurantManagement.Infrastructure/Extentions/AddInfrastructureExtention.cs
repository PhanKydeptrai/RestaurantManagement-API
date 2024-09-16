using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Extentions;

public static class AddInfrastructureExtention
{
    public static IServiceCollection AddInfrastructureExtentions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<RestaurantManagementDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("MyDB")));
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<RestaurantManagementDbContext>());
        return services;
    }
}
