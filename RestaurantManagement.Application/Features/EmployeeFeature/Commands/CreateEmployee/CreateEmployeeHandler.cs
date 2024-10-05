using MediatR;
using FluentValidation.Results;
using RestaurantManagement.Domain.IRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;


namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.CreateEmployee
{
    public class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, Result<bool>>
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
        public async Task<Result<bool>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var result = new Result<bool>
            {
                ResultValue = false
            };

            // validate
            CreateEmployeeCommandValidator validator = new CreateEmployeeCommandValidator();
            var validationResult = validator.Validate(request); // Phải dùng thư viện using FluentValidation.Results;
            if (validationResult.IsValid)
            {
                validationResult.Errors.Select(e => e.ErrorMessage).ToArray();

                return result;
            }
            // email already exist
            var isEmailExist = await _employeeRepository.IsEmployyeEmailExist(request.Email);
            if (isEmailExist)
            {
                result.Errors = new string[] { "Email is already exist" };
                return result;
            }
            var isPhoneNumberExist = await _employeeRepository.IsEmployeePhoneExist(request.PhoneNumber);
            if (isPhoneNumberExist)
            {
                result.Errors = new string[] { "Phone number is already exist" };
                return result;
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
            result.ResultValue = true;
            result.IsSuccess = true;

            return result;
        }
    }
}
