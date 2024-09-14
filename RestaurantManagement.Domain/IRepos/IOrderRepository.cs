using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface IOrderRepository
{
    //CRUD
    Task<IEnumerable<Order>> GetAllOrders();
    Task<Order?> GetOrderById(Guid id);
    Task AddOrder(Order order);
    void UpdateOrder(Order order);
    void DeleteOrder(Order order); 
    //Queries
    IQueryable<Order> GetOrdersQueryable();
    Task<bool> IsStatusExist(string status); //kiểm tra xem status nào tồn tại hay không?
    Task<IEnumerable<Order>> GetOrdersByCustomerId(Guid id);
    Task<IEnumerable<Order>> GetOrdersByOrderStatus(string status);
}
