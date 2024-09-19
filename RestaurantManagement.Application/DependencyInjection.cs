using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;

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
        services.AddScoped<IBookingRepository>(provider => provider.GetRequiredService<IBookingRepository>());
        return services;
    }

}
