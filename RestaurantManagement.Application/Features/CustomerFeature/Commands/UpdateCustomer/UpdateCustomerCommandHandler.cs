using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CustomerFeature.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result>
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
    public async Task<Result> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {

        //validation
        var validator = new UpdateCustomerCommandValidator(_customerRepository);
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error(e.ErrorCode, e.ErrorMessage))
                .ToArray();

            return Result.Failure(errors);
        }

        var user = await _context.Customers
            .Include(a => a.User)
            .Where(a => a.CustomerId == request.CustomerId)
            .Select(a => a.User).FirstAsync();

        if (user == null)
        {
            Error[] error = { new Error("Customer", "Customer not found") };
            return Result.Failure(error);
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Phone = request.PhoneNumber;
        user.UserImage = request.UserImage ?? user.UserImage;
        //? Should we add a email field to the UpdateCustomerCommand class?
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
