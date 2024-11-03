using FluentValidation;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.BookingFeature.Commands.TableArrangement;

public class TableArrangementCommandValidator : AbstractValidator<TableArrangementCommand>
{
    public TableArrangementCommandValidator(
        IBookingRepository bookingRepository, 
        ITableRepository tableRepository,
        IApplicationDbContext applicationDbContext)
    {
        RuleFor(a => a.BookingId)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => bookingRepository.IsBookingStatusValid(a).Result == true)
            .WithMessage("Booking status is invalid.");
            
        RuleFor(a => a.TableId)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => tableRepository.IsTableExist(a).Result == true)
            .WithMessage("table is not found")
            .Must(a => tableRepository.IsTableAvailable(a).Result == true)
            .WithMessage("Table is not available")
            .Custom( async (tableId, context) =>
            {
                var bookingId = context.InstanceToValidate.BookingId;
                //Lấy số lượng khách hàng từ booking
                int numberOfCustomer = bookingRepository.GetNumberOfCustomers(bookingId).Result;
                //Lấy số lượng bàn từ table
                int tableCapacity = tableRepository.GetTableCapacity(tableId).Result;

                if(numberOfCustomer > tableCapacity)
                {
                    context.AddFailure("Capacity of this table is not enough");
                } 
            });
        


    }
}

