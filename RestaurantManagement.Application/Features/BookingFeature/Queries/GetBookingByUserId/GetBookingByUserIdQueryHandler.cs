using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.DTOs.BookingDtos;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.BookingFeature.Queries.GetBookingByUserId;

public class GetBookingByUserIdQueryHandler(
    IBookingRepository bookingRepository,
    ICustomerRepository customerRepository) : IQueryHandler<GetBookingByUserIdQuery, BookingResponse[]>
{
    public async Task<Result<BookingResponse[]>> Handle(GetBookingByUserIdQuery request, CancellationToken cancellationToken)
    {
        //Validate request
        var validator = new GetBookingByUserIdQueryValidator(customerRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result<BookingResponse[]>.Failure(errors!);
        }

        var booking = await bookingRepository.GetBookingResponseByUserId(Ulid.Parse(request.id));

        return Result<BookingResponse[]>.Success(booking);
    }
}
