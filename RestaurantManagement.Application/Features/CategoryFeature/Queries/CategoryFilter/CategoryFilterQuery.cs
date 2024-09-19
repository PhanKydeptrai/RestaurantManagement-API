using MediatR;

namespace RestaurantManagement.Application.Features.CategoryFeature.Queries.CategoryFilter;

public record CategoryFilterQuery(string? searchTerm, int page, int pageSize) : IRequest<PagedList<CategoryResponse>>;

public record CategoryResponse()
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string CategoryStatus { get; set; }
    public string Image { get; set; } //NOTE: This will be changed to byte[] in the future
    public string? Desciption { get; set; }
}