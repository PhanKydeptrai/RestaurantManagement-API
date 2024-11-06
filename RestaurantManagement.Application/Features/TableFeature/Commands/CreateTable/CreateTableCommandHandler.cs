using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.CreateTable;

public class CreateTableCommandHandler(
    ISystemLogRepository systemLogRepository,
    IApplicationDbContext context,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateTableCommand>
{
    public async Task<Result> Handle(CreateTableCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new CreateTableCommandValidator();
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
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

        await context.Tables.AddRangeAsync(tableArray);

        //Decode jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //Create System Log
        await systemLogRepository.CreateSystemLog(new SystemLog
        {
            SystemLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetail = $"Tạo {request.quantity} loại {request.tableTypeId} thành bán",
            UserId = Ulid.Parse(userId)
        });
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
