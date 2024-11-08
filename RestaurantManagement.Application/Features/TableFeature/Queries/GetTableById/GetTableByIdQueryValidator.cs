using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.TableFeature.Queries.GetTableById;

public class GetTableByIdQueryValidator : AbstractValidator<GetTableByIdQuery>
{
    public GetTableByIdQueryValidator(ITableRepository tableRepository)
    {
        RuleFor(p => p.id)
            .Must(a => tableRepository.IsTableExist(int.Parse(a)).Result)
            .WithMessage("Table does not exist.")
            .When(a => int.TryParse(a.id, out _))
            
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => int.TryParse(a, out _))
            .WithMessage("{PropertyName} must be a number.");
    }
}
