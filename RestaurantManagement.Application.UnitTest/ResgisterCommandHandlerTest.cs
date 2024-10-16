using FluentAssertions;
using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using Moq;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Features.AccountFeature.Commands.Register;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.UnitTest;

public class ResgisterCommandHandlerTest
{
    private readonly Mock<ICustomerRepository> _customerRepository;
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<IApplicationDbContext> _context;
    private readonly Mock<IFluentEmail> _fluentEmail;
    private readonly Mock<IEmailVerify> _emailVerify;

    public ResgisterCommandHandlerTest()
    {
        _emailVerify = new();
        _fluentEmail = new();
        _context = new();
        _unitOfWork = new();
        _userRepository = new();
        _customerRepository = new();
        // Mock the EmailVerificationTokens property
        _context.Setup(c => c.EmailVerificationTokens).Returns(new Mock<DbSet<EmailVerificationToken>>().Object);
    }

    [Fact]  
    public async Task Hanlde_Should_Return_Failure_When_RegisterCommand_Is_Invalid()
    {
        //Arrange
        var command = new RegisterCommand("Ky", "Phan", "kyp1gmail.com", "123", "0777637527", "Male");
        var handler = new RegisterCommandHandler(
            _customerRepository.Object,
            _unitOfWork.Object,
            _userRepository.Object, 
            _context.Object,
            _fluentEmail.Object, 
            _emailVerify.Object);
        
        //Act
        var result = await handler.Handle(command, default);
        //Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Hanlde_Should_Return_Sucess_When_RegisterCommand_Is_Valid()
    {
        //Arrange
        var command = new RegisterCommand("Ky", "Phan", "kyp1@gmail.com", "123456789", "0777637527", "Male");
        var handler = new RegisterCommandHandler(
            _customerRepository.Object,
            _unitOfWork.Object,
            _userRepository.Object,
            _context.Object, _fluentEmail.Object,
            _emailVerify.Object);

        //Act
        var result = await handler.Handle(command, default);
        //Assert
        result.IsSuccess.Should().BeTrue();
    }
}
