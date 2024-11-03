using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.BookingDtos;

namespace RestaurantManagement.Application.Features.BookingFeature.Queries.GetBookingByUserId;

public record GetBookingByUserIdQuery(Ulid id) : IQuery<BookingResponse[]>;
