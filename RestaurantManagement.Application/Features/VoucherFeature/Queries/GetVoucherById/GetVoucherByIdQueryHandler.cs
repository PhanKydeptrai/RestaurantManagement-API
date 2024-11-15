using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.VoucherFeature.Queries.GetVoucherById;

public class GetVoucherByIdQueryHandler(IVoucherRepository voucherRepository) : IQueryHandler<GetVoucherByIdQuery, Voucher>
{
    public async Task<Result<Voucher>> Handle(GetVoucherByIdQuery request, CancellationToken cancellationToken)
    {
        //TODO: validate
        //validate
        var validator = new GetVoucherByIdQueryValidator(voucherRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result<Voucher>.Failure(errors);
        }
        Voucher? voucher = await voucherRepository.GetVoucherById(Ulid.Parse(request.id));

        return Result<Voucher>.Success(voucher);
    }
}
