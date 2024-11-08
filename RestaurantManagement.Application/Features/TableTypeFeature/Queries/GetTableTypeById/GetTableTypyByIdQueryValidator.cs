using FluentValidation;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Queries.GetTableTypeById;

public class GetTableTypyByIdQueryValidator : AbstractValidator<GetTableTypyByIdQuery>
{
    public GetTableTypyByIdQueryValidator()
    {
        RuleFor(x => x.id)
            .NotNull()
            .WithMessage("Id is required")
            .NotEmpty()
            .WithMessage("Id is required")
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("Id is not valid");
    }
}
