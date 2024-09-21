using MediatR;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Response;

namespace RestaurantManagement.Application.Features.CustomerFeature.Commands.CreateCustomer;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<bool>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUserRepository _userRepository;   
    private readonly IUnitOfWork _unitOfWork;
    public CreateCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork, IUserRepository userRepository)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }
    public async Task<Result<bool>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        Result<bool> result = new Result<bool>();

        //Validation
        CreateCustomerCommandValidator validator = new CreateCustomerCommandValidator(_customerRepository);
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            //result.Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToArray();
            result.Errors = validationResult.Errors
                                .Select(x => x.ErrorMessage)
                                .ToArray();
            return result;
        }

        //Create Customer
        User user = new User
        {
            UserId = Guid.NewGuid(),
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Password = request.Password,
            FirstName = request.FirstName,
            Status = "hd",
            LastName = request.LastName,
            Gender = request.Gender
        };

        Customer customer = new Customer
        {
            CustomerId = Guid.NewGuid(),
            UserId = user.UserId,
            CustomerType = "dk"
        };

        await _userRepository.CreateUser(user);
        await _customerRepository.CreateCustomer(customer);
        await _unitOfWork.SaveChangesAsync();
        result.ResultValue = true;
        result.IsSuccess = true;
        return result;
        
    }
}


