using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.ChangeTable;

public class ChangeTableCommandHandler : ICommandHandler<ChangeTableCommand>
{
    private readonly ITableRepository _tableRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDbContext _context;

    public ChangeTableCommandHandler(
        ITableRepository tableRepository,
        IUnitOfWork unitOfWork,
        IApplicationDbContext context)
    {
        _tableRepository = tableRepository;
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<Result> Handle(ChangeTableCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new ChangeTableCommandValidator(_tableRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }
        
        //lấy order của bàn cũ
        var order = await _context.Tables
            .Include(a => a.Orders)
            .Where(a => a.TableId == int.Parse(request.oldtableId))
            .Select(a => a.Orders.FirstOrDefault(a => a.PaymentStatus == "Unpaid"))
            .FirstOrDefaultAsync();

        //cập nhật order sang bàn mới
        order.TableId = int.Parse(request.newTableId.ToString());
        //cập nhật trạng thái bàn
        var oldTable = await _context.Tables.FindAsync(int.Parse(request.oldtableId));
        var newTable = await _context.Tables.FindAsync(int.Parse(request.newTableId.ToString()));

        oldTable.ActiveStatus = "Empty";
        newTable.ActiveStatus = "Occupied";

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
