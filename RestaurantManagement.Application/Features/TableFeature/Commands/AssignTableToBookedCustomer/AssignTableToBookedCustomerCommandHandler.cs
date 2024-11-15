using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.AssignTableToBookedCustomer;

public class AssignTableToBookedCustomerCommandHandler(
    ITableRepository tableRepository,
    IUnitOfWork unitOfWork,
    IApplicationDbContext context) : ICommandHandler<AssignTableToBookedCustomerCommand>
{
    public async Task<Result> Handle(AssignTableToBookedCustomerCommand request, CancellationToken cancellationToken)
    {
        //TODO: validate
        //validate
        var validator = new AssignTableToBookedCustomerCommandValidator(tableRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }

        //lấy thông tin booking

        var booking = await context.Bookings.Include(a => a.BookingDetails)
            .Where(a => a.BookingDetails.Any(b => b.TableId == int.Parse(request.tableId) && a.BookingStatus == "Seated"))
            .FirstOrDefaultAsync();

        booking.BookingStatus = "Occupied";

        await tableRepository.UpdateActiveStatus(int.Parse(request.tableId), "Occupied");
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
