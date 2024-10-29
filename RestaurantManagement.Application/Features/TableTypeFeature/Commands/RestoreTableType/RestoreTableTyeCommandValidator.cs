using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Commands.RestoreTableType;

public class RestoreTableTyeCommandValidator : AbstractValidator<RestoreTableTyeCommand>
{
    public RestoreTableTyeCommandValidator(ITableTypeRepository tableTypeRepository)
    {
        RuleFor(x => x.id)
            .NotNull()
            .WithMessage("Id is required.")
            .NotEmpty()
            .WithMessage("Id is required.")
            .Must(a => tableTypeRepository.IsTableTypeExist(a).Result == true)
            .WithMessage("Table type does not exist.")
            .Must(a => tableTypeRepository.GetTableTypeStatus(a).Result == "InActive")
            .WithMessage("Table type is Active.");
    }
}
