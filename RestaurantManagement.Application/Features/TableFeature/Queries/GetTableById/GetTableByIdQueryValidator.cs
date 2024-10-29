using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.TableFeature.Queries.GetTableById;

public class GetTableByIdQueryValidator : AbstractValidator<GetTableByIdQuery>
{
    public GetTableByIdQueryValidator(ITableRepository tableRepository)
    {
        RuleFor(p => p.id)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => tableRepository.IsTableExist(a).Result)
            .WithMessage("Table does not exist.");
    }
}
