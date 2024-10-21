using Microsoft.AspNetCore.Http;
using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Commands.CreateTableType;

public record CreateTableTypeCommand(
    string TableTypeName, 
    IFormFile? Image, 
    decimal TablePrice,
    string? Description,
    string token) : ICommand;
