using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Commands.DeleteTableType;
//Fluent validation
public class DeleteTableTypeCommandValidator : AbstractValidator<DeleteTableTypeCommand>
{
    public DeleteTableTypeCommandValidator(ITableTypeRepository tableTypeRepository)
    {
        RuleFor(x => x.id)
            
            .Must(a => tableTypeRepository.IsTableTypeExist(Ulid.Parse(a)).Result == true)
            .WithMessage("Table type does not exist.")
            .Must(a => tableTypeRepository.GetTableTypeStatus(Ulid.Parse(a)).Result == "Active")
            .WithMessage("Table type is InActive.")
            .When(a => Ulid.TryParse(a.id, out _))

            .NotNull()
            .WithMessage("Id is required.")
            .NotEmpty()
            .WithMessage("Id is required.")
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("Id is not valid.");
    }
}
