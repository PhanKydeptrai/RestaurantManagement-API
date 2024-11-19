using Microsoft.AspNetCore.Http;
using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Commands.UpdateTableType;


//NOTE: Đã xử lý Untrusted data
public record UpdateTableTypeCommand(
    string TableTypeId, 
    string TableTypeName, 
    IFormFile? Image, 
    string TablePrice,
    string? TableCapacity,
    string? Description,
    string token
) : ICommand;
