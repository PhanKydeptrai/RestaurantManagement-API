using FluentEmail.Core;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CustomerFeature.Commands.RestoreCustomer;

public class RestoreCustomerCommandHandler : ICommandHandler<RestoreCustomerCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IFluentEmail _fluentEmail;
    private readonly ICustomerRepository _customerRepository;
    public RestoreCustomerCommandHandler(
        IApplicationDbContext context,
        ICustomerRepository customerRepository,
        IFluentEmail fluentEmail)
    {
        _context = context;
        _customerRepository = customerRepository;
        _fluentEmail = fluentEmail;
    }

    public async Task<Result> Handle(RestoreCustomerCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new RestoreCustomerCommandValidator(_customerRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        //Decode jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //lấy mail khách hàng
        var customerInfo = await _customerRepository.GetCustomerById(Ulid.Parse(userId));

        // Gửi email thông báo
        bool emailSent = false;
        int retryCount = 0;
        int maxRetries = 5;

        do
        {
            try
            {
                await _fluentEmail.To(customerInfo.Email).Subject("Nhà hàng Nhum nhum - Thông báo vô hiệu hoá tài khoản")
                    .Body($"Nhà hàng Nhum Nhum xin trân trọng thông báo: <br> Tài khoản của quý khách đã được khôi phục. <br>Chúc quý khách một ngày tốt lành. <br> Nhum Nhum xin chân thành cảm ơn.", isHtml: true)
                    .SendAsync();
                emailSent = true;
            }
            catch
            {
                retryCount++;
                if (retryCount >= maxRetries)
                {
                    return Result.Failure(new[] { new Error("Email", "Failed to send email") });
                }
            }
        }
        while (!emailSent && retryCount < maxRetries);

        //
        //Create System Log
        await _context.CustomerLogs.AddAsync(new CustomerLog
        {
            CustomerLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetails = $"khôi phục tài khoản cho khách hàng {customerInfo.FirstName} {customerInfo.LastName} ID: {customerInfo.UserId}",
            UserId = Ulid.Parse(userId)
        });
        return Result.Success();

    }
}
