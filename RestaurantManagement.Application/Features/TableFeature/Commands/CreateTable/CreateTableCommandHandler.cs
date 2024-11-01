using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.CreateTable;

public class CreateTableCommandHandler : ICommandHandler<CreateTableCommand>
{
    private readonly ISystemLogRepository _systemLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDbContext _context;
    public CreateTableCommandHandler(
        ISystemLogRepository systemLogRepository,
        IApplicationDbContext context,
        IUnitOfWork unitOfWork)
    {
        _systemLogRepository = systemLogRepository;
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CreateTableCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new CreateTableCommandValidator();
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(a => new Error(a.ErrorCode, a.ErrorMessage)).ToArray();
            return Result.Failure(errors);
        }

        //create table
        var tableArray = new Table[request.quantity];


        for(int i = 0; i < request.quantity; i++)
        {
            tableArray[i] = new Table
            {
                TableId = PrimaryKeyGenerator.GeneratePrimaryKey(),
                TableTypeId = request.tableTypeId,
                TableStatus = "Active",
                ActiveStatus = "Empty"
            };
        }

        await _context.Tables.AddRangeAsync(tableArray);

        //Decode jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //Create System Log
        await _systemLogRepository.CreateSystemLog(new SystemLog
        {
            SystemLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetail = $"Tạo {request.quantity} loại {request.tableTypeId} thành bán",
            UserId = Ulid.Parse(userId)
        });
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
