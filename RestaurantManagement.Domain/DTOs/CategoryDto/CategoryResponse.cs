namespace RestaurantManagement.Domain.DTOs.CategoryDto;

public record CategoryResponse()
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string CategoryStatus { get; set; }
    public string Image { get; set; } //NOTE: This will be changed to byte[] in the future
    public string? Desciption { get; set; }
}
