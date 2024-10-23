namespace RestaurantManagement.Domain.DTOs.TableTypeDto;
public record TableTypeResponse(
    Ulid TableTypeId, 
    string TableTypeName, 
    string Status, 
    string? ImageUrl,
    decimal TablePrice,
    string? Description);
