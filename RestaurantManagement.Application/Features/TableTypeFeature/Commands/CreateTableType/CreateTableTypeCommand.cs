using Microsoft.AspNetCore.Http;
using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Commands.CreateTableType;
//NOTE: Đã lý Unstrusted data
public record CreateTableTypeCommand(
    string TableTypeName, 
    IFormFile? Image, 
    string TablePrice,
    string TableCapacity,
    string? Description,
    string token) : ICommand;
