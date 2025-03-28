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

        services.AddFluentEmail(configuration["Email:SenderEmail"], configuration["Email:Sender"])
                .AddSmtpSender(new SmtpClient(configuration["Email:Host"], int.Parse(configuration["Email:Port"])));
        // try
        // {
        //     SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
        //     {
        //         Port = int.Parse(configuration["Port"]!),
        //         Credentials = new NetworkCredential(configuration["SenderEmail"], configuration["AppPasswords"]),
        //         EnableSsl = true // Sử dụng SSL
        //     };
        //     // Console.WriteLine("SenderEmail: " + smtpClient.Credentials.GetCredential(smtpClient.Host, smtpClient.Port, "smtp").UserName);


        //     services.AddFluentEmail(configuration["SenderEmail"], configuration["Sender"])
        //             .AddSmtpSender(smtpClient);
        // }
        // catch (Exception)
        // {
        //     services.AddFluentEmail(configuration["Email:SenderEmail"], configuration["Email:Sender"])
        //             .AddSmtpSender(new SmtpClient(configuration["Email:Host"], int.Parse(configuration["Email:Port"])));
        // }

        return services;
    }

}
