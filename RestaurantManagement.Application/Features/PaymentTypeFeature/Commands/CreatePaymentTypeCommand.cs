using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.PaymentTypeFeature.Commands;

public record CreatePaymentTypeCommand(string PaymentTypeName) : ICommand;
