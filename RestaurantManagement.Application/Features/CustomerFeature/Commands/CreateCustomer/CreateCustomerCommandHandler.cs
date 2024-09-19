

using FluentValidation.Results;
using MediatR;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Response;

namespace RestaurantManagement.Application.Features.CustomerFeature.Commands.CreateCustomer;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateCustomerCommandHandler(IUserRepository userRepository, ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<bool>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var result = new Result<bool>
        {
            IsSuccess = false,
            ResultValue = false,
            Errors = new List<string>()
        };

        //Validate
        CreateCustomerCommandValidator validator = new CreateCustomerCommandValidator(_customerRepository);
        ValidationResult validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                result.Errors.Add(error.ErrorMessage);
            }
            return result;
        }

        //Create User
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserId = Guid.NewGuid(),
            Email = request.Email,
            Status = "Active",
            Password = request.Password,
            PhoneNumber = request.PhoneNumber
        };

        //Create Customer
        var customer = new Customer
        {
            CustomerId = Guid.NewGuid(),
            UserId = user.UserId
        };

        await _userRepository.CreateUser(user);
        await _customerRepository.CreateCustomer(customer);
        await _unitOfWork.SaveChangesAsync();
        result.ResultValue = result.IsSuccess = true;

        return result;
    }
}


