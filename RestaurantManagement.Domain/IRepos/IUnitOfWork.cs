namespace RestaurantManagement.Domain.IRepos;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}
