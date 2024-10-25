using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.DeleteTable;

public class DeleteTableCommandValidator : AbstractValidator<DeleteTableCommand>
{
    public DeleteTableCommandValidator(ITableRepository tableRepository)
    {
        RuleFor(a => a.id)
            .NotNull()
            .WithMessage("Table id is required")
            .NotEmpty()
            .WithMessage("Table id is required")
            .Must(a => tableRepository.IsTableExist(a).Result == true)
            .WithMessage("Table is not exist") //Kiểm tra bàn có tồn tại hay không?
            .Must(a => tableRepository.GetTableStatus(a).Result == "empty")
            .WithMessage("Table is not empty")
            .Must(a => tableRepository.GetActiveStatus(a).Result == "active")
            .WithMessage("Table is still inactive");//Kiểm tra bàn có trạng thái là empty hay không?
    }
}
