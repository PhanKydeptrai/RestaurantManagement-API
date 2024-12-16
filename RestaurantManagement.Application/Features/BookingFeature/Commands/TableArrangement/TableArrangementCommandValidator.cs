using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.BookingFeature.Commands.TableArrangement;

public class TableArrangementCommandValidator : AbstractValidator<TableArrangementCommand>
{

    public TableArrangementCommandValidator(
        IBookingRepository bookingRepository,
        ITableRepository tableRepository,
        IApplicationDbContext dbcontext)
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
            .Must(a => tableRepository.IsTableExistAndActive(int.Parse(a.ToString()!)).Result == true)
            .WithMessage("table is not found")
            .Must(a => tableRepository.IsTableAvailable(int.Parse(a.ToString()!)).Result == true)
            .WithMessage("Table is not available")
            .Custom(async (tableId, context) =>
            {
                var bookingId = context.InstanceToValidate.BookingId;
                //Lấy số lượng khách hàng từ booking
                int numberOfCustomer = bookingRepository.GetNumberOfCustomers(Ulid.Parse(bookingId)).Result;
                //Lấy số lượng bàn từ table
                int tableCapacity = tableRepository.GetTableCapacity(int.Parse(tableId.ToString()!)).Result;

                if (numberOfCustomer > tableCapacity)
                {
                    context.AddFailure("Capacity of this table is not enough");
                }
            })
            // .Custom(async (tableId, context) =>
            // {
            //     var bookingId = context.InstanceToValidate.BookingId;
            //     var recentBooking = await dbcontext.Bookings.FindAsync(Ulid.Parse(bookingId));
            //     //Kiểm tra xem bàn đã được book chưa
            //     var bookingInfo = await dbcontext.Bookings.Include(a => a.BookingDetails)
            //         .Where(
            //             a => a.BookingStatus == "Seated" 
            //             && 
            //             a.BookingDetails.FirstOrDefault().TableId == int.Parse(tableId.ToString()!)) //đã xếp bàn
            //         .ToListAsync();
                
            //     foreach(var info in bookingInfo)
            //     {
            //         //So sánh ngày book hiện tại với ngaỳ book của booking đã xếp bàn
            //         //Nếu cùng ngày thì kiểm tra giờ
            //         if(info.BookingDate.ToString("dd/MM/yyyy") == recentBooking.BookingDate.ToString("dd/MM/yyyy"))
            //         {
            //             if(info.BookingTime == recentBooking.BookingTime) //cùng giờ thì từ chối
            //             {
            //                 context.AddFailure("Table is already booked");
            //             }

            //         }
            //     }
            // })
            .When(a => a.TableId != null &&  int.TryParse(a.TableId.ToString(), out _) == true)

            .NotNull()
            .WithMessage("{PropertyName} is null.")
            .NotEmpty()
            .WithMessage("{PropertyName} is empty.")
            .Must(a => a != null && int.TryParse(a.ToString(), out _))
            .WithMessage("Table id is invalid");


        
    }
    
}

