using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.OrderDto;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.OrderFeature.Queries.GetAllOrder;

public class GetAllOrderQueryHandler(
    IApplicationDbContext context,
    IOrderRepository orderRepository) : IQueryHandler<GetAllOrderQuery, PagedList<OrderResponse>>
{
    public async Task<Result<PagedList<OrderResponse>>> Handle(GetAllOrderQuery request, CancellationToken cancellationToken)
    {
        //TODO: validate
        var validator = new GetAllOrderQueryValidator();
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result<PagedList<OrderResponse>>.Failure(errors);
        }
        
        var orderQuery = context.Orders
            .Include(a => a.Customer)
            .Include(a => a.OrderDetails)
            .ThenInclude(b => b.Meal)
            .AsQueryable();

        //Search
        if (!string.IsNullOrEmpty(request.searchTerm))
        {
            orderQuery = orderQuery.Where(x => x.TableId.ToString().Contains(request.searchTerm));
        }

        //Filter
        if (!string.IsNullOrEmpty(request.filterPaymentStatus)) //filter by payment status
        {
            orderQuery = orderQuery.Where(x => x.PaymentStatus == request.filterPaymentStatus);
        }

        if (!string.IsNullOrEmpty(request.filterUserId)) //filter by user id
        {
            orderQuery = orderQuery.Where(x => x.Customer.UserId == Ulid.Parse(request.filterUserId));
        }

        if (!string.IsNullOrEmpty(request.filterTableId)) //filter by table id
        {
            orderQuery = orderQuery.Where(x => x.TableId.ToString() == request.filterTableId);
        }

        //sort
        Expression<Func<Order, object>> keySelector = request.sortColumn?.ToLower() switch
        {
            "orderid" => x => x.OrderId,
            _ => x => x.OrderId
        };

        if (request.sortOrder?.ToLower() == "desc")
        {
            orderQuery = orderQuery.OrderByDescending(keySelector);
        }
        else
        {
            orderQuery = orderQuery.OrderBy(keySelector);
        }

        //paged
        var categories = orderQuery.Select(a => new OrderResponse(
                a.TableId,
                a.OrderId,
                a.PaymentStatus,
                (int)a.Total,
                a.OrderDetails.Select(b => new OrderDetailResponse(
                    b.OrderDetailId,
                    b.MealId,
                    b.Meal.MealName,
                    (int)b.Meal.Price,
                    b.Meal.ImageUrl,
                    b.Quantity,
                    (int)b.UnitPrice
                )).ToArray())).AsQueryable();


        var categoriesList = await PagedList<OrderResponse>.CreateAsync(categories, request.page ?? 1, request.pageSize ?? 10);

        return Result<PagedList<OrderResponse>>.Success(categoriesList);

    }
}
