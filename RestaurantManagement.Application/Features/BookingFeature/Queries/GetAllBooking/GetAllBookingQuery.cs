using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.BookingDtos;

namespace RestaurantManagement.Application.Features.BookingFeature.Queries.GetAllBooking;

public record GetAllBookingQuery(
    string? filterPaymentStatus,
    string? searchTerm,
    string? sortColumn,
    string? sortOrder,
    int? page,
    int? pageSize) : IQuery<PagedList<BookingResponse>>;