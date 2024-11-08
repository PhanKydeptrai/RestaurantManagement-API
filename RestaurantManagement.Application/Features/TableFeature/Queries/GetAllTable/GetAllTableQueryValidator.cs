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

    }
}
