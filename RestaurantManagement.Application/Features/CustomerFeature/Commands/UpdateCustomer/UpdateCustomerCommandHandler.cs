using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.DTOs.Common;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.CustomerFeature.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerCommandHandler(
        IUnitOfWork unitOfWork, 
        ICustomerRepository customerRepository, 
        IApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _customerRepository = customerRepository;
        _context = context;
    }


    //TODO: Refactor phương thức này để sử dụng repository
    public async Task<Result<bool>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var result = new Result<bool>();
        //validation
        var validator = new UpdateCustomerCommandValidator(_customerRepository);
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            result.Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
            return result;
        }

        var user = await _context.Customers
            .Include(a => a.User)
            .Where(a => a.CustomerId == request.CustomerId)
            .Select(a => a.User).FirstAsync();
    
        if(user != null)
        {
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Phone = request.PhoneNumber;
            user.UserImage = request.UserImage ?? user.UserImage;
            //? Should we add a email field to the UpdateCustomerCommand class?
            await _unitOfWork.SaveChangesAsync();
            result.ResultValue = result.IsSuccess = true;
        }

        return result;
    }
}
