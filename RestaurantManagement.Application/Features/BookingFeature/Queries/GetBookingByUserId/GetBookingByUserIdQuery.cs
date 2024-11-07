using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.BookingDtos;

namespace RestaurantManagement.Application.Features.BookingFeature.Queries.GetBookingByUserId;

public record GetBookingByUserIdQuery(string id) : IQuery<BookingResponse[]>;
