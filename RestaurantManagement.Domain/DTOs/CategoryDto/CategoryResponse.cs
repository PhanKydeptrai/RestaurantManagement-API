namespace RestaurantManagement.Domain.DTOs.CategoryDto;

//TODO: need rafactor
public record CategoryResponse()
{
    public Ulid CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string CategoryStatus { get; set; }
    public string Image { get; set; } //NOTE: This will be changed to byte[] in the future
}
