using FluentValidation;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.AssignTableToCustomer;

#region Stable version
public class AssignTableToCustomerCommandValidator : AbstractValidator<AssignTableToCustomerCommand>
{
    public AssignTableToCustomerCommandValidator(ITableRepository tableRepository)
    {
        RuleFor(p => p.id)
            .Must(a => tableRepository.GetActiveStatus(int.Parse(a)).Result == "Empty")
            .WithMessage("This table is not empty.")
            .When(a => int.TryParse(a.id, out _))

            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => int.TryParse(a, out _))
            .WithMessage("{PropertyName} must be a number.");

    }
}
#endregion


#region In development

// public AssignTableToCustomerCommandValidator(ITableRepository tableRepository, IApplicationDbContext dbcontext)
//     {
//         RuleFor(p => p.id)
//             .Custom(async (tableId, context) =>
//             {
//                 var tableStatus = await tableRepository.GetActiveStatus(int.Parse(tableId));
//                 if (tableStatus != "Empty" && tableStatus != "Booked")
//                 {
//                     TimeOnly timeOnlyNow = TimeOnly.FromDateTime(DateTime.Now.AddHours(-3));
//                     DateOnly dateOnlyNow = DateOnly.FromDateTime(DateTime.Now);
//                     var check = await dbcontext.Tables
//                         .Where(a => a.TableId == int.Parse(tableId))
//                         .Include(a => a.BookingDetails)
//                         .ThenInclude(a => a.Booking)
//                         .Select(a => a.BookingDetails
//                         .Where(a => a.Booking.BookingStatus == "Booked" 
//                         && a.Booking.BookingTime > timeOnlyNow 
//                         && a.Booking.BookingDate == dateOnlyNow))
//                         .AnyAsync();
//                     if(check == true)
//                     {
//                         context.AddFailure("This table is not empty.");
//                     }
//                 }
//             })
//             .When(a => int.TryParse(a.id, out _))
//             .NotNull()
//             .WithMessage("{PropertyName} is required.")
//             .NotEmpty()
//             .WithMessage("{PropertyName} is required.")
//             .Must(a => int.TryParse(a, out _))
//             .WithMessage("{PropertyName} must be a number.");

//     }
// }
#endregion