using Microsoft.AspNetCore.Http;
using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Commands.UpdateTableType;


public record UpdateTableTypeCommand(
    Ulid TableTypeId, 
    string TableTypeName, 
    IFormFile? Image, 
    decimal TablePrice,
    string? Description,
    string token
) : ICommand;
