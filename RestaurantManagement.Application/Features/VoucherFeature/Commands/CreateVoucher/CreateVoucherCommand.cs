using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.VoucherFeature.Commands.CreateVoucher;

public record CreateVoucherCommand(
    string VoucherName, 
    decimal MaxDiscount, 
    decimal VoucherCondition,
    DateTime StartDate,
    DateTime ExpiredDate,
    string? Description,
    string token) : ICommand;
    
public record CreateVoucherRequest(
    string VoucherName, 
    decimal MaxDiscount, 
    decimal VoucherCondition,
    DateTime StartDate,
    DateTime ExpiredDate,
    string? Description);
