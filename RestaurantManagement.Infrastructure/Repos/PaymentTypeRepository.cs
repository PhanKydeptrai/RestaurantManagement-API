using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class PaymentTypeRepository(RestaurantManagementDbContext context) : IPaymentTypeRepository
{
    
    public async Task<bool> IsPaymentTypeExist(string paymentTypeName)
    {
        return await context.PaymentTypes.AnyAsync(p => p.Name == paymentTypeName);
    }
}
