using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Quartz;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Authentication;
using RestaurantManagement.Infrastructure.BackgroundJob;
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
        // services.AddScoped<ISystemLogRepository, SystemLogRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        // services.AddScoped<ISystemLogRepository, SystemLogRepository>();
        services.AddScoped<IEmailVerify, EmailVerify>();
        services.AddScoped<ITableTypeRepository, TableTypeRepository>();
        services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
        services.AddScoped<IVoucherRepository, VoucherRepository>();
        services.AddScoped<IBillRepository, BillRepository>();

        services.AddQuartz(options =>
        {
            options.UseMicrosoftDependencyInjectionJobFactory();

            var jobKey_VoucherBackgroundJob = JobKey.Create(nameof(VoucherBackgroundJob));
            
            options.AddJob<VoucherBackgroundJob>(jobKey_VoucherBackgroundJob)
                    .AddTrigger(trigger => 
                        trigger.ForJob(jobKey_VoucherBackgroundJob)
                    .WithSimpleSchedule(schedule => schedule.WithIntervalInHours(12).RepeatForever()));
            
            var jobKey_UpdateTableStatusForBooking = JobKey.Create(nameof(UpdateTableStatusForBooking));

            options.AddJob<UpdateTableStatusForBooking>(jobKey_UpdateTableStatusForBooking)
                    .AddTrigger(trigger => 
                        trigger.ForJob(jobKey_UpdateTableStatusForBooking)
                    .WithSimpleSchedule(schedule => schedule.WithIntervalInMinutes(5).RepeatForever()));

            var jobkey_UpdateBookingStatusBackgroundJob = JobKey.Create(nameof(UpdateBookingStatusBackgroundJob));

            options.AddJob<UpdateBookingStatusBackgroundJob>(jobkey_UpdateBookingStatusBackgroundJob)
                    .AddTrigger(trigger => 
                        trigger.ForJob(jobkey_UpdateBookingStatusBackgroundJob)
                    .WithSimpleSchedule(schedule => schedule.WithIntervalInMinutes(5).RepeatForever()));
        });

        services.AddQuartzHostedService();

        services.AddHttpContextAccessor();
        return services;
    }
}
