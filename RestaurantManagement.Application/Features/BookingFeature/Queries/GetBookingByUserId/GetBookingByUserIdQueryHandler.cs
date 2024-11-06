using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.DTOs.BookingDtos;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.BookingFeature.Queries.GetBookingByUserId;

public class GetBookingByUserIdQueryHandler : IQueryHandler<GetBookingByUserIdQuery, BookingResponse[]>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ICustomerRepository _customerRepository;
    public GetBookingByUserIdQueryHandler(
        IBookingRepository bookingRepository, 
        ICustomerRepository customerRepository)
    {
        _bookingRepository = bookingRepository;
        _customerRepository = customerRepository;
    }

    public async Task<Result<BookingResponse[]>> Handle(GetBookingByUserIdQuery request, CancellationToken cancellationToken)
    {
       
        var validator = new GetBookingByUserIdQueryValidator(_bookingRepository, _customerRepository);
        if (!await ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result<BookingResponse[]>.Failure(errors);
        }
        var booking = await _bookingRepository.GetBookingResponseByUserId(request.id);

        return Result<BookingResponse[]>.Success(booking);
    }
}
