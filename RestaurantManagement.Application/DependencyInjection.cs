using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RestaurantManagement.Application.Behaviors;

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
        
        // Đăng ký pipeline behaviors cho log
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
        //Đăng ký pipeline behaviors cho validation
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        return services;



    }

}
