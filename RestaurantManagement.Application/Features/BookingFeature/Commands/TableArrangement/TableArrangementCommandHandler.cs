using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.BookingFeature.Commands.TableArrangement;

public class TableArrangementCommandHandler : ICommandHandler<TableArrangementCommand>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IFluentEmail  _fluentEmail;
    private readonly ITableRepository _tableRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDbContext _context;

    public TableArrangementCommandHandler(
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork,
        ITableRepository tableRepository,
        IApplicationDbContext context,
        IFluentEmail fluentEmail)
    {
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _tableRepository = tableRepository;
        _context = context;
        _fluentEmail = fluentEmail;
    }

    public async Task<Result> Handle(TableArrangementCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new TableArrangementCommandValidator(_bookingRepository, _tableRepository, _context);
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(a => new Error(a.ErrorCode, a.ErrorMessage)).ToArray();
            return Result.Failure(errors);
        }

        //Cập nhật trạng thái đã xếp bàn cho booking
        await _bookingRepository.UpdateBookingStatus(request.BookingId);
        var userEmail = await _context.Bookings
            .Include(a => a.Customer)
            .ThenInclude(a => a.User)
            .Where(a => a.BookId == request.BookingId)
            .Select(a => a.Customer.User.Email)
            .FirstOrDefaultAsync();
            
        //Tạo một booking detail
        var bookingDetail = new BookingDetail
        {
            BookId = request.BookingId,
            BookingDetailId = Ulid.NewUlid(),
            TableId = request.TableId
        };
        
        await _context.BookingDetails.AddAsync(bookingDetail);
        //Cập nhật active status của bàn
        await _tableRepository.UpdateActiveStatus(request.TableId, "Booked");
        await _unitOfWork.SaveChangesAsync();
        
        //thông báo cho người dùng
        var bookingInfo = await _bookingRepository.GetBookingResponseById(request.BookingId);
        await _fluentEmail.To(userEmail)
            .Subject("Thông báo")
            .Body($"Nhà hàng đã xác nhận được thông tin đặt bàn của bạn.<br> Đây là thông tin đặt bàn của bạn: <br> Mã Booking của bạn là: {bookingInfo.BookId} <br>Tên: {bookingInfo.LastName + " " + bookingInfo.FirstName} <br> Ngày:{bookingInfo.BookingDate}<br> Thời gian: {bookingInfo.BookingTime} <br> Email: {bookingInfo.Email} <br> Số điện thoại:{bookingInfo.Phone} <br> Số bàn của bạn là: {bookingDetail.TableId} " , isHtml: true)
            .SendAsync();

        return Result.Success();
    }
}
