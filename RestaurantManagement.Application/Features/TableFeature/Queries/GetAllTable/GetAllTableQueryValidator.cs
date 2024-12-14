using System.Data;
using FluentValidation;

namespace RestaurantManagement.Application.Features.TableFeature.Queries.GetAllTable;

public class GetAllTableQueryValidator : AbstractValidator<GetAllTableQuery>
{
    public GetAllTableQueryValidator()
    {
        RuleFor(a => a.filterTableType)
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("Table type is invalid")
            .When(a => !string.IsNullOrEmpty(a.filterTableType));
        
        RuleFor(a => a.searchTerm)
            .Must(a => int.TryParse(a, out _))
            .WithMessage("Search term is should be a number")
            .When(a => !string.IsNullOrEmpty(a.searchTerm));

    }
}
