using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.VoucherFeature.Queries.GetVoucherById;

public class GetVoucherByIdQueryHandler : IQueryHandler<GetVoucherByIdQuery, Voucher>
{
    private readonly IVoucherRepository _voucherRepository;

    public GetVoucherByIdQueryHandler(IVoucherRepository voucherRepository)
    {
        _voucherRepository = voucherRepository;
    }

    public async Task<Result<Voucher>> Handle(GetVoucherByIdQuery request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new GetVoucherByIdQueryValidator(_voucherRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result<Voucher>.Failure(errors);
        }
        Voucher? voucher = await _voucherRepository.GetVoucherById(request.id);

        return Result<Voucher>.Success(voucher);
    }
}
