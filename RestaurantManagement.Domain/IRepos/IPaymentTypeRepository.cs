namespace RestaurantManagement.Domain.IRepos;

public interface IPaymentTypeRepository
{
    Task<bool> IsPaymentTypeExist(string paymentTypeName);
}
