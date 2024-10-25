using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.UpdateStatusToBooked;

public record UpdateStatusToBookedCommand(Ulid id, string token) : ICommand;
