using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.CreateTable;

public class CreateTableCommandHandler(
    IApplicationDbContext context,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateTableCommand>
{
    public async Task<Result> Handle(CreateTableCommand request, CancellationToken cancellationToken)
    {

        //Validate request
        var validator = new CreateTableCommandValidator();
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        //create table
        var tableArray = new Table[(int)request.quantity];
        Ulid tableTypeId = Ulid.Parse(request.tableTypeId);

        for (int i = 0; i < (int)request.quantity; i++)
        {
            tableArray[i] = new Table
            {
                TableId = PrimaryKeyGenerator.GeneratePrimaryKey(),
                TableTypeId = tableTypeId,
                TableStatus = "Active",
                ActiveStatus = "Empty"
            };
        }

        await context.Tables.AddRangeAsync(tableArray);


        //TODO: Cập nhật system log
        #region Decode jwt and system log
        // //Decode jwt
        // var claims = JwtHelper.DecodeJwt(request.token);
        // claims.TryGetValue("sub", out var userId);

        // //Create System Log
        // await systemLogRepository.CreateSystemLog(new SystemLog
        // {
        //     SystemLogId = Ulid.NewUlid(),
        //     LogDate = DateTime.Now,
        //     LogDetail = $"Tạo {request.quantity} loại {request.tableTypeId} thành bán",
        //     UserId = Ulid.Parse(userId)
        // });
        #endregion
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}


#region Stable code
// public class CreateTableCommandHandler(
//     ISystemLogRepository systemLogRepository,
//     IApplicationDbContext context,
//     IUnitOfWork unitOfWork) : ICommandHandler<CreateTableCommand>
// {
//     public async Task<Result> Handle(CreateTableCommand request, CancellationToken cancellationToken)
//     {

//         //Validate request
//         var validator = new CreateTableCommandValidator();
//         Error[]? errors = null;
//         var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
//         if (!isValid)
//         {
//             return Result.Failure(errors!);
//         }

//         //create table
//         var tableArray = new Table[request.quantity];
//         Ulid tableTypeId = Ulid.Parse(request.tableTypeId);

//         for (int i = 0; i < request.quantity; i++)
//         {
//             tableArray[i] = new Table
//             {
//                 TableId = PrimaryKeyGenerator.GeneratePrimaryKey(),
//                 TableTypeId = tableTypeId,
//                 TableStatus = "Active",
//                 ActiveStatus = "Empty"
//             };
//         }

//         await context.Tables.AddRangeAsync(tableArray);


//         //TODO: Cập nhật system log
//         #region Decode jwt and system log
//         // //Decode jwt
//         // var claims = JwtHelper.DecodeJwt(request.token);
//         // claims.TryGetValue("sub", out var userId);

//         // //Create System Log
//         // await systemLogRepository.CreateSystemLog(new SystemLog
//         // {
//         //     SystemLogId = Ulid.NewUlid(),
//         //     LogDate = DateTime.Now,
//         //     LogDetail = $"Tạo {request.quantity} loại {request.tableTypeId} thành bán",
//         //     UserId = Ulid.Parse(userId)
//         // });
//         #endregion
//         await unitOfWork.SaveChangesAsync();
//         return Result.Success();
//     }
// }
#endregion
