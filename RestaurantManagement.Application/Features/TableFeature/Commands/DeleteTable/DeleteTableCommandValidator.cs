using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.DeleteTable;

public class DeleteTableCommandValidator : AbstractValidator<DeleteTableCommand>
{
    public DeleteTableCommandValidator(ITableRepository tableRepository)
    {
        RuleFor(a => a.id)
            .Must(a => tableRepository.IsTableExist(int.Parse(a)).Result == true)
            .WithMessage("Table is not exist") //Kiểm tra bàn có tồn tại hay không?
            .Must(a => tableRepository.GetActiveStatus(int.Parse(a)).Result == "Empty")
            .WithMessage("Table is not empty")
            .Must(a => tableRepository.GetTableStatus(int.Parse(a)).Result == "Active")
            .WithMessage("Table is still InActive")//Kiểm tra bàn có trạng thái là empty hay không?
            .When(a => int.TryParse(a.id, out _))

            .NotNull()
            .WithMessage("Table id is required")
            .NotEmpty()
            .WithMessage("Table id is required")
            .Must(a => int.TryParse(a, out _))
            .WithMessage("Table id must be a number");
            
    }
}
