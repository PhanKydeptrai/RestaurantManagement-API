using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.RemoveCategory;

public class RemoveCategoryCommandHandler : IRequestHandler<RemoveCategoryCommand, bool>
{
    private readonly ICategoryRepository _categoryRepository;

    private readonly IUnitOfWork _unitOfWork;

    public RemoveCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RemoveCategoryCommand request, CancellationToken cancellationToken)
    {
        //Check status of category
        if (!_categoryRepository.CheckStatusOfCategory(request.Id).Result)
        {
            return false;
        }
        //soft delete
        _categoryRepository.SoftDeleteCategory(request.Id);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
