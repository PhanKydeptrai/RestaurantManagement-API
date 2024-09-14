using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface IOrderDetailRepository
{
    //CRUD
    Task<OrderDetail> CreateOrderDetail(OrderDetail orderDetail);
    Task<ICollection<OrderDetail>> GetOrderDetails();
    Task<OrderDetail> GetOrderDetailById(Guid id);
    void UpdateOrderDetail(OrderDetail orderDetail);
    void DeleteOrderDetail(OrderDetail orderDetail);
    //Queries
    IQueryable<OrderDetail> GetQueryableOrderDetails();
    Task<ICollection<OrderDetail>> GetOrderDetailsByOrderId(Guid id);
    
}
