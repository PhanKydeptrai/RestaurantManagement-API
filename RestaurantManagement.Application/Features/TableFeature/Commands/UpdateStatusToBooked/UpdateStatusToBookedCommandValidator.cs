using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.UpdateStatusToBooked;

public class UpdateStatusToBookedCommandValidator : AbstractValidator<UpdateStatusToBookedCommand>
{
    public UpdateStatusToBookedCommandValidator(ITableRepository tableRepository)
    {
        RuleFor(a => a.id)
            .NotNull()
            .WithMessage("Table id is required")
            .NotEmpty()
            .WithMessage("Table id is required")
            .Must(a => tableRepository.IsTableExist(a).Result == true)
            .WithMessage("Table is not exist")
            .Must(a => tableRepository.GetTableStatus(a).Result == "Empty")
            .WithMessage("Table is not empty");
        
        
            
    }
}
