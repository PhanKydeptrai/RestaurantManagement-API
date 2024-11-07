using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.BookingDtos;

namespace RestaurantManagement.Application.Features.BookingFeature.Queries.GetBookingByBookingId;

public record GetBookingByBookingIdQuery(string id) : IQuery<BookingResponse>;
