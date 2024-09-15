using Microsoft.Extensions.DependencyInjection;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Repos;

namespace RestaurantManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        //Đăng ký dependency injection cho các repository tại đây
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        return services;
    }
}
