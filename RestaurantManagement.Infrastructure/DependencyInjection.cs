using Microsoft.Extensions.DependencyInjection;
using Quartz;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Authentication;
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
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
        services.AddScoped<IMealRepository, MealRepository>();
        services.AddScoped<IMealRepository, MealRepository>();
        services.AddScoped<ITableRepository, TableRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IBookingDetailRepository, BookingDetailRepository>();
        services.AddScoped<ISystemLogRepository, SystemLogRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ISystemLogRepository, SystemLogRepository>();
        services.AddScoped<IEmailVerify, EmailVerify>();
        services.AddScoped<ITableTypeRepository, TableTypeRepository>();
        services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
        services.AddScoped<IVoucherRepository, VoucherRepository>();
        services.AddScoped<IBillRepository, BillRepository>();

        services.AddQuartz(options =>
        {
            options.UseMicrosoftDependencyInjectionJobFactory();
        });

        services.AddQuartzHostedService();

        services.AddHttpContextAccessor();
        return services;
    }
}
