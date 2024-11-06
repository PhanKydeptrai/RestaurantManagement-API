using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.BookingFeature.Commands.CancelCreateBooking;

public class CancelBookingCommandHandler : ICommandHandler<CancelBookingCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDbContext _context;
    private readonly IBookingRepository _bookingRepository;
    private readonly ITableRepository _tableRepository;

    public CancelBookingCommandHandler(
        IUnitOfWork unitOfWork,
        IBookingRepository bookingRepository,
        ITableRepository tableRepository,
        IApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _bookingRepository = bookingRepository;
        _tableRepository = tableRepository;
        _context = context;
    }

    public async Task<Result> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new CancelBookingCommandValidator(_bookingRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }

        //cancel booking
        if(await _bookingRepository.IsBookingCompleted(request.id)) //Nếu đã xếp bàn
        {
            var bookingDetails = await _context.BookingDetails
                .Where(a => a.BookId == request.id)
                .Select(a => a.TableId)
                .ToArrayAsync();

            foreach (var tableId in bookingDetails)
            {
                await _tableRepository.UpdateActiveStatus(tableId, "Empty");
            }
        }


        await _bookingRepository.CancelBooking(request.id);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
