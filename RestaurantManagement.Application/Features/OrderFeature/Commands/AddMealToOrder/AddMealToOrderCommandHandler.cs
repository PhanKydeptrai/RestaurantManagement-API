using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.AddMealToOrder;

public class AddMealToOrderCommandHandler(
    IUnitOfWork unitOfWork,
    ITableRepository tableRepository,
    IMealRepository mealRepository,
    IApplicationDbContext context) : ICommandHandler<AddMealToOrderCommand>
{
    public async Task<Result> Handle(AddMealToOrderCommand request, CancellationToken cancellationToken)
    {
        // Kiểm tra bàn đã có order hay chưa
        // Nếu chưa => tạo mới order và tạo orderdetail
        // Nếu đã có order => kiểm tra món đã có trong  
        // order chưa, nếu có thì cập nhật số lượng, nếu chưa thì tạo mới orderdetail


        //Validate request
        var validator = new AddMealToOrderCommandValidator(tableRepository, mealRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        //Lấy order chưa thanh toán => đang ăn
        var order = await context.Tables
            .Include(a => a.Orders)
            .Where(a => a.TableId == int.Parse(request.TableId))
            .Select(a => a.Orders.FirstOrDefault(a => a.PaymentStatus == "Unpaid"))
            .FirstOrDefaultAsync();

        
        try //Lấy bill chưa thanh toán của bàn đang ăn (có booking)
        {
            var bill = await context.Tables
                .Include(a => a.BookingDetails)
                .ThenInclude(a => a.Booking)
                .ThenInclude(a => a.Bill)
                .Where(a => a.TableId == int.Parse(request.TableId) && a.BookingDetails.Any(a => a.Booking.Bill.PaymentStatus == "Unpaid"))
                .Select(a => a.BookingDetails.FirstOrDefault().Booking.Bill)
                .FirstOrDefaultAsync();

            bill.OrderId = order.OrderId; //Cập nhật orderid cho bill
        }
        catch (Exception) { }

        var mealPrice = await context.Meals.AsNoTracking() //Lấy giá món ăn
            .Where(a => a.MealId == Ulid.Parse(request.MealId))
            .Select(a => a.Price)
            .FirstOrDefaultAsync();

        if (order != null) //Kiểm tra order đã tồn tại hay chưa 
        {
            //kiểm tra món đã có trong order chưa
            var orderDetail = await context.Orders
                .Include(a => a.OrderDetails)
                .ThenInclude(a => a.Order)
                .Where(a => a.TableId == int.Parse(request.TableId))
                .Select(a => a.OrderDetails.FirstOrDefault(a => a.MealId == Ulid.Parse(request.MealId)))
                .FirstOrDefaultAsync();

            if (orderDetail != null) //Nếu tồn tại
            {
                orderDetail.Quantity += request.Quantity; //Cập nhật số lượng
                orderDetail.Order.Total = orderDetail.Order.Total - orderDetail.UnitPrice; //Trừ đi tổng tiền cũ
                orderDetail.UnitPrice = orderDetail.Quantity * mealPrice; //Cập nhật tổng tiền
                orderDetail.Order.Total += orderDetail.UnitPrice;
            }
            else
            {
                var neworderDetail = new OrderDetail
                {
                    OrderDetailId = Ulid.NewUlid(),
                    OrderId = order.OrderId,
                    MealId = Ulid.Parse(request.MealId),
                    Quantity = request.Quantity,
                    UnitPrice = request.Quantity * mealPrice,
                    Note = string.Empty
                };

                order.Total += neworderDetail.UnitPrice;
                await context.OrderDetails.AddAsync(neworderDetail);
            }
        }
        else
        {
            //Kiểm tra bàn có được đặt hay không
            Ulid? customerId = await tableRepository.GetCustomerIdByTableId(int.Parse(request.TableId));

            order = new Order
            {
                OrderId = Ulid.NewUlid(),
                Note = string.Empty,
                Total = mealPrice * request.Quantity,
                OrderTime = DateTime.Now,
                CustomerId = null,
                TableId = int.Parse(request.TableId),
                PaymentStatus = "Unpaid"
            };

            var orderDetail = new OrderDetail
            {
                OrderDetailId = Ulid.NewUlid(),
                OrderId = order.OrderId,
                MealId = Ulid.Parse(request.MealId),
                Quantity = request.Quantity,
                UnitPrice = request.Quantity * mealPrice,
                Note = string.Empty
            };

            if (customerId != null) //Nếu có đặt bàn sẽ lấy id khách hàng
            {
                order.CustomerId = customerId;
            }



            await context.Orders.AddAsync(order);
            await context.OrderDetails.AddAsync(orderDetail);
        }


        #region Decode jwt and system log
        //decode token
        var claims = JwtHelper.DecodeJwt(request.Token);
        claims.TryGetValue("sub", out var userId);
        var userInfo = await context.Users.FindAsync(Ulid.Parse(userId));
        //Create System Log
        await context.OrderLogs.AddAsync(new OrderLog
        {
            OrderLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetails = $"{userInfo.FirstName + " " + userInfo.LastName} thêm món {request.MealId} vào order {order.OrderId}",
            UserId = Ulid.Parse(userId)
        });
        #endregion
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
