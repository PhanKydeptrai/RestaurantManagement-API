using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CustomerFeature.Commands.DeleteCustomer;

public class DeleteCustomerCommandHandler : ICommandHandler<DeleteCustomerCommand>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IFluentEmail _fluentEmail;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDbContext _context;

    public DeleteCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IFluentEmail fluentEmail,
        IApplicationDbContext context)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _fluentEmail = fluentEmail;
        _context = context;
    }

    public async Task<Result> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        
        var validator = new DeleteCustomerCommandValidator(_customerRepository);
        //Validate request
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        var userEmail = await _context.Users.Where(x => x.UserId == Ulid.Parse(request.userId)).Select(a => a.Email).FirstOrDefaultAsync();
        await _customerRepository.DeleteCustomer(Ulid.Parse(request.userId));
        
        //Gửi mail thông báo
        
        //TODO: Xử lý lỗi gửi mail
        await _fluentEmail.To(userEmail).Subject("Nhà hàng Nhum nhum - Thông báo vô hiệu hoá tài khoản")
                .Body($"Nhà hàng Nhum Nhum xin trân trọng thông báo: <br> Tài khoản của quý khách đã bị vô hiệu hoá do những vi phạm. <br> Nếu có sai sót xin quý khách vui lòng gọi vào số hotline 0903159123 của nhà hàng. <br> Nhum Nhum xin chân thành cảm ơn.", isHtml: true)
                .SendAsync();
    
        

        //Lưu thay đổi 
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
