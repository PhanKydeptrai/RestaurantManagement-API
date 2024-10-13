using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;


namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.CreateEmployee
{
    public class CreateEmployeeHandler : ICommandHandler<CreateEmployeeCommand>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateEmployeeHandler(IEmployeeRepository employeeRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            
            // validate
            var validator = new CreateEmployeeCommandValidator(_employeeRepository);
            var validationResult = await validator.ValidateAsync(request); // Phải dùng thư viện using FluentValidation.Results;
            if (validationResult.IsValid)
            {
                Error[] errors = validationResult.Errors
                    .Select(a => new Error(a.ErrorCode, a.ErrorMessage))
                    .ToArray();

                return Result.Failure(errors);
            }
            
            // create new
            var user = new User
            {
                UserId = Ulid.NewUlid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.Password,
                Phone = request.PhoneNumber,
                Status = "Activated",
            };

            var employee = new Employee
            {
                EmployeeId = Ulid.NewUlid(),
                Role = request.Role,
                EmployeeStatus = "Active",
                UserId = user.UserId,
            };

            await _userRepository.CreateUser(user);
            await _employeeRepository.CreateEmployee(employee);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
    }
}
