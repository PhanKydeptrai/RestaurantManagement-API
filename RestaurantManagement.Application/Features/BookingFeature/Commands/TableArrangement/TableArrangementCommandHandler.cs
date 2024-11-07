using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.BookingFeature.Commands.TableArrangement;

public class TableArrangementCommandHandler(
    IBookingRepository bookingRepository,
    IUnitOfWork unitOfWork,
    ITableRepository tableRepository,
    IApplicationDbContext context,
    IFluentEmail fluentEmail) : ICommandHandler<TableArrangementCommand>
{
    public async Task<Result> Handle(TableArrangementCommand request, CancellationToken cancellationToken)
    {
        
        var validator = new TableArrangementCommandValidator(bookingRepository, tableRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }
        
        //Cập nhật trạng thái đã xếp bàn cho booking
        await bookingRepository.UpdateBookingStatus(Ulid.Parse(request.BookingId));
        var userEmail = await context.Bookings
            .Include(a => a.Customer)
            .ThenInclude(a => a.User)
            .Where(a => a.BookId == Ulid.Parse(request.BookingId))
            .Select(a => a.Customer.User.Email)
            .FirstOrDefaultAsync();
            
        //Tạo một booking detail
        var bookingDetail = new BookingDetail
        {
            BookId = Ulid.Parse(request.BookingId),
            BookingDetailId = Ulid.NewUlid(),
            TableId = int.Parse(request.TableId)
        };
        
        await context.BookingDetails.AddAsync(bookingDetail);
        //Cập nhật active status của bàn
        await tableRepository.UpdateActiveStatus(int.Parse(request.TableId), "Booked");
        await unitOfWork.SaveChangesAsync();
        
        //thông báo cho người dùng
        var bookingInfo = await bookingRepository.GetBookingResponseById(Ulid.Parse(request.BookingId));
        await fluentEmail.To(userEmail)
            .Subject("Thông báo")
            .Body($"Nhà hàng đã xác nhận được thông tin đặt bàn của bạn.<br> Đây là thông tin đặt bàn của bạn: <br> Mã Booking của bạn là: {bookingInfo.BookId} <br>Tên: {bookingInfo.LastName + " " + bookingInfo.FirstName} <br> Ngày:{bookingInfo.BookingDate}<br> Thời gian: {bookingInfo.BookingTime} <br> Email: {bookingInfo.Email} <br> Số điện thoại:{bookingInfo.Phone} <br> Số bàn của bạn là: {bookingDetail.TableId} " , isHtml: true)
            .SendAsync();

        return Result.Success();
    }
}
