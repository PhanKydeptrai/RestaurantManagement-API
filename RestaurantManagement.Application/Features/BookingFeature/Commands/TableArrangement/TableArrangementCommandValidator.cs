using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.BookingFeature.Commands.TableArrangement;

public class TableArrangementCommandValidator : AbstractValidator<TableArrangementCommand>
{
    public TableArrangementCommandValidator(
        IBookingRepository bookingRepository, 
        ITableRepository tableRepository)
    {
        RuleFor(a => a.BookingId)
            .Must(a => bookingRepository.IsBookingStatusValid(Ulid.Parse(a)).Result == true)
            .WithMessage("Booking status is invalid.")
            .When(a => Ulid.TryParse(a.BookingId, out _) == true)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => Ulid.TryParse(a, out _) == true)
            .WithMessage("Booking id is invalid");
            
        RuleFor(a => a.TableId)
            .Must(a => tableRepository.IsTableExistAndActive(int.Parse(a)).Result == true)
            .WithMessage("table is not found")
            .Must(a => tableRepository.IsTableAvailable(int.Parse(a)).Result == true)
            .WithMessage("Table is not available")
            .Custom( async (tableId, context) =>
            {
                var bookingId = context.InstanceToValidate.BookingId;
                //Lấy số lượng khách hàng từ booking
                int numberOfCustomer = bookingRepository.GetNumberOfCustomers(Ulid.Parse(bookingId)).Result;
                //Lấy số lượng bàn từ table
                int tableCapacity = tableRepository.GetTableCapacity(int.Parse(tableId)).Result;

                if(numberOfCustomer > tableCapacity)
                {
                    context.AddFailure("Capacity of this table is not enough");
                } 
            })
            .When(a => int.TryParse(a.TableId, out _) == true)
            
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => int.TryParse(a, out _) == true)
            .WithMessage("Table id is invalid");
        


    }
}

