namespace RestaurantManagement.Application.Features.CustomerFeature.DTOs;

//TODO: need rafactor
public record CustomerResponse()
{
    public Ulid CustomerId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string Gender { get; set; }
    public byte[]? UserImage { get; set; }
}
