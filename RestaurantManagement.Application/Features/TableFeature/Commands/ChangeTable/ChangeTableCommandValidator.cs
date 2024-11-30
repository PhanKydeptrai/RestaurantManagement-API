using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.ChangeTable;

public class ChangeTableCommandValidator : AbstractValidator<ChangeTableCommand>
{
    public ChangeTableCommandValidator(ITableRepository tableRepository)
    {
        RuleFor(a => a.oldtableId)
            .NotNull()
            .WithMessage("Old table id is required")
            .NotEmpty()
            .WithMessage("Old table id is required")
            .Must(a => !string.IsNullOrEmpty(a) && int.TryParse(a, out _))
            .WithMessage("Old table id must be a number")
            .Must(a => tableRepository.IsTableExistAndActive(int.Parse(a)).Result)
            .WithMessage("This table is not exist or not active")
            .Must(a => tableRepository.IsTableOccupied(int.Parse(a)).Result)
            .WithMessage("This table is not occupied");

        RuleFor(a => a.newTableId)
            .NotNull()
            .WithMessage("Old table id is required")
            .NotEmpty()
            .WithMessage("Old table id is required")
            .Must(a => !string.IsNullOrEmpty(a.ToString()) && int.TryParse(a.ToString(), out _))
            .WithMessage("Old table id must be a number")
            .Custom(async (a, context) =>
            {
                if (int.Parse(a.ToString()) == int.Parse(context.InstanceToValidate.oldtableId))
                {
                    context.AddFailure("New table id must be different from old table id");
                }
            })
            .Must(a => tableRepository.IsTableExistAndActive(int.Parse(a.ToString())).Result)
            .WithMessage("This table is not exist or not active")
            .Must(a => tableRepository.GetTableStatus(int.Parse(a.ToString())).Result != "Booked")
            .WithMessage("This table is booked")
            .Must(a => tableRepository.GetTableStatus(int.Parse(a.ToString())).Result != "Occupied")
            .WithMessage("This table is occupied");
    }
}
