using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.UpdateStatusToBooked;

public record UpdateStatusToBookedCommand(int id, string token) : ICommand;
