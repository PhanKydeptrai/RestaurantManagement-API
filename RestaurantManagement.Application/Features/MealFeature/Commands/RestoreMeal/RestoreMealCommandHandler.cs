using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.RestoreMeal;
public class RestoreMealCommandHandler : ICommandHandler<RestoreMealCommand>
{
    private readonly IMealRepository _mealRepository;
    private readonly IUnitOfWork _unitOfWork;
    public RestoreMealCommandHandler(
        IMealRepository mealRepository, 
        IUnitOfWork unitOfWork)
    {
        _mealRepository = mealRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RestoreMealCommand request, CancellationToken cancellationToken)
    {
        var validator = new RestoreMealCommandValidator(_mealRepository);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if(!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(a => new Error(a.ErrorCode, a.ErrorMessage))
                .ToArray();
            return Result.Failure(errors);

        }
        await _mealRepository.RestoreMeal(request.id);

        return Result.Success();
    }
}

