using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestaurantManagement.API.Behavior;
using System.Net;
using System.Net.Mail;


namespace RestaurantManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Đăng ký MediatR
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            configuration.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
        });

        // Đăng ký FluentValidation
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        //Cấu hình FluentValidation
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;


        // Đăng ký FluentEmail

        SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = int.Parse(configuration["Port"]!), // Cổng SMTP cho Gmail với TLS
            Credentials = new NetworkCredential(configuration["RestaurantEmail"], configuration["AppPasswords"]), // Mật khẩu ứng dụng ở đây
            EnableSsl = true // Sử dụng SSL
        };

        // services.AddFluentEmail(configuration["Email:SenderEmail"], configuration["Email:Sender"])
        //         .AddSmtpSender(new SmtpClient(configuration["Email:Host"], int.Parse(configuration["Email:Port"])));

        services.AddFluentEmail(configuration["SenderEmail"], configuration["Sender"])
                .AddSmtpSender(smtpClient);

        return services;
    }

}
