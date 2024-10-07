using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.RemoveCategory;

public class RemoveCategoryCommandHandler : ICommandHandler<RemoveCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;

    private readonly IUnitOfWork _unitOfWork;

    public RemoveCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveCategoryCommand request, CancellationToken cancellationToken)
    {
        //Check status of category
        if (!_categoryRepository.CheckStatusOfCategory(request.Id).Result)
        {
           
            return Result.Failure(new[] { new Error("Category", "Category not found") });
        }
        //soft delete
        _categoryRepository.SoftDeleteCategory(request.Id);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
