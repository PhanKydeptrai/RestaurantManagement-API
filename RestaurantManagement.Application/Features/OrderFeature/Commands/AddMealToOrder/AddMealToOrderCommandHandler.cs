using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.AddMealToOrder;

public class AddMealToOrderCommandHandler : ICommandHandler<AddMealToOrderCommand>
{
    private readonly ITableRepository _tableRepository;
    private readonly IMealRepository _mealRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDbContext _context;

    public AddMealToOrderCommandHandler(
        IUnitOfWork unitOfWork,
        ITableRepository tableRepository,
        IMealRepository mealRepository,
        IApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _tableRepository = tableRepository;
        _mealRepository = mealRepository;
        _context = context;
    }


    public async Task<Result> Handle(AddMealToOrderCommand request, CancellationToken cancellationToken)
    {
        // Kiểm tra bàn đã có order hay chưa
        // Nếu chưa => tạo mới order và tạo orderdetail
        // Nếu đã có order => kiểm tra món đã có trong  
        // order chưa, nếu có thì cập nhật số lượng, nếu chưa thì tạo mới orderdetail

        //validate
        var validator = new AddMealToOrderCommandValidator(_tableRepository, _mealRepository);
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(a => new Error(a.ErrorCode, a.ErrorMessage)).ToArray();
            return Result.Failure(errors);
        }

        //Lấy order chưa thanh toán => đang ăn
        var order = await _context.Tables
            .Include(a => a.Orders)
            .Where(a => a.TableId == request.TableId)
            .Select(a => a.Orders.FirstOrDefault(a => a.PaymentStatus == "Unpaid"))
            .FirstOrDefaultAsync();

        var mealPrice = await _context.Meals.AsNoTracking() //Lấy giá món ăn
            .Where(a => a.MealId == request.MealId)
            .Select(a => a.Price)
            .FirstOrDefaultAsync();



        if (order != null) //Kiểm tra order đã tồn tại hay chưa 
        {
            //kiểm tra món đã có trong order chưa
            var orderDetail = await _context.Orders
                .Include(a => a.OrderDetails)
                .ThenInclude(a => a.Order)
                .Where(a => a.TableId == request.TableId)
                .Select(a => a.OrderDetails.FirstOrDefault(a => a.MealId == request.MealId))
                .FirstOrDefaultAsync();

            if (orderDetail != null) //Nếu tồn tại
            {
                orderDetail.Quantity += request.Quantity; //Cập nhật số lượng
                orderDetail.UnitPrice = orderDetail.Quantity * mealPrice; //Cập nhật tổng tiền
                orderDetail.Order.Total = orderDetail.UnitPrice;
            }
            else
            {
                var neworderDetail = new OrderDetail
                {
                    OrderDetailId = Ulid.NewUlid(),
                    OrderId = order.OrderId,
                    MealId = request.MealId,
                    Quantity = request.Quantity,
                    UnitPrice = request.Quantity * mealPrice,
                    Note = string.Empty
                };

                order.Total += neworderDetail.UnitPrice;
                await _context.OrderDetails.AddAsync(neworderDetail);
            }

        }
        else
        {
            //Kiểm tra bàn có được đặt hay không
            Ulid? customerId = await _tableRepository.GetCustomerIdByTableId(request.TableId);

            order = new Order
            {
                OrderId = Ulid.NewUlid(),
                Note = string.Empty,
                Total = mealPrice * request.Quantity,
                OrderTime = DateTime.Now,
                CustomerId = null,
                TableId = request.TableId,
                PaymentStatus = "Unpaid"
            };

            var orderDetail = new OrderDetail
            {
                OrderDetailId = Ulid.NewUlid(),
                OrderId = order.OrderId,
                MealId = request.MealId,
                Quantity = request.Quantity,
                UnitPrice = request.Quantity * mealPrice,
                Note = string.Empty
            };



            if (customerId != null) //Nếu có đặt sẽ lấy id khách hàng
            {
                order.CustomerId = customerId;
            }

            await _context.Orders.AddAsync(order);
            await _context.OrderDetails.AddAsync(orderDetail);
        }

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
