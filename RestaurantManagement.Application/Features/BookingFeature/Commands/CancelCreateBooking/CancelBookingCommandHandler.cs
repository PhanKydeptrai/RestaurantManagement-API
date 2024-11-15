using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.BookingFeature.Commands.CancelCreateBooking;

public class CancelBookingCommandHandler(
    IUnitOfWork unitOfWork,
    IBookingRepository bookingRepository,
    ITableRepository tableRepository,
    IApplicationDbContext context) : ICommandHandler<CancelBookingCommand>
{
    public async Task<Result> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        //TODO: validate
        //validate
        var validator = new CancelBookingCommandValidator(bookingRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }

        //cancel booking
        if(await bookingRepository.IsBookingCompleted(Ulid.Parse(request.id))) //Nếu đã xếp bàn
        {
            var bookingDetails = await context.BookingDetails
                .Where(a => a.BookId == Ulid.Parse(request.id))
                .Select(a => a.TableId)
                .ToArrayAsync();

            foreach (var tableId in bookingDetails)
            {
                await tableRepository.UpdateActiveStatus(tableId, "Empty");
            }
        }


        await bookingRepository.CancelBooking(Ulid.Parse(request.id));
        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
