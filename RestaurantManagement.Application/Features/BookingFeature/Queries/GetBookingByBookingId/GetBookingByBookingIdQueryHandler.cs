using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.BookingDtos;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.BookingFeature.Queries.GetBookingByBookingId;

public class GetBookingByBookingIdQueryHandler : IQueryHandler<GetBookingByBookingIdQuery, BookingResponse>
{
    private readonly IBookingRepository _bookingRepository;
    public GetBookingByBookingIdQueryHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<Result<BookingResponse>> Handle(GetBookingByBookingIdQuery request, CancellationToken cancellationToken)
    {
        
        var booking = await _bookingRepository.GetBookingResponseById(request.id);
        if (booking == null)
        {
            var error = new[] { new Error("Booking", "Booking not found") };
            return Result<BookingResponse>.Failure(error);
        }
        return Result<BookingResponse>.Success(booking);
    }
}
