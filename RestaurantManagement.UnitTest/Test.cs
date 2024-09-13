using Moq;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.UnitTest;

public class Test
{
    private readonly Mock<ICustomerRepository> _mockCustomerRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    public Test()
    {
        _mockCustomerRepository = new Mock<ICustomerRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
    }

    [Fact]
    public void Test1()
    {
        var email = "email@example.com";
        _mockCustomerRepository.Setup(x => x.IsCustomerEmailExist(email)).ReturnsAsync(false);
        
    }
}
