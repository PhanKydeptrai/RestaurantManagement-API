using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;
using RestaurantManagement.Application.Abtractions;


namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.CreateEmployee
{
    public class CreateEmployeeHandler : ICommand<CreateEmployeeCommand>
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
            var validationResult = validator.Validate(request); // Phải dùng thư viện using FluentValidation.Results;
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
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.Password,
                Phone = request.PhoneNumber,
                Status = "Active",
            };
            var employee = new Employee
            {
                Role = request.Role,
            };
            await _userRepository.CreateUser(user);
            await _employeeRepository.CreateEmployee(employee);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
    }
}
