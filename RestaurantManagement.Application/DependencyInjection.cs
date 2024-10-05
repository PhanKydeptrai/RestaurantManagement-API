using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace RestaurantManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Đăng ký MediatR
        services.AddMediatR(configuration => {
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });
        // Đăng ký FluentValidation
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        return services;
    }

}
