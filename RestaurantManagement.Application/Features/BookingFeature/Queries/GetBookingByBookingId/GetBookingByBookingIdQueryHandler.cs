using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.DTOs.BookingDtos;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.BookingFeature.Queries.GetBookingByBookingId;

public class GetBookingByBookingIdQueryHandler(IBookingRepository bookingRepository) : IQueryHandler<GetBookingByBookingIdQuery, BookingResponse>
{
    public async Task<Result<BookingResponse>> Handle(GetBookingByBookingIdQuery request, CancellationToken cancellationToken)
    {
        var validator = new GetBookingByBookingIdQueryValidator();
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result<BookingResponse>.Failure(errors);
        }

        var booking = await bookingRepository.GetBookingResponseById(Ulid.Parse(request.id));
        if (booking == null)
        {
            var error = new[] { new Error("Booking", "Booking not found") };
            return Result<BookingResponse>.Failure(error);
        }
        return Result<BookingResponse>.Success(booking);
    }
}
