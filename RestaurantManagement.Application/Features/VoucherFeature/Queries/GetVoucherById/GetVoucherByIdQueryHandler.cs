using RestaurantManagement.Application.Abtractions;
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
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(a => new Error(a.ErrorCode, a.ErrorMessage)).ToArray();
            return Result<Voucher>.Failure(errors);
        }
        Voucher? voucher = await _voucherRepository.GetVoucherById(request.id);

        return Result<Voucher>.Success(voucher);
    }
}
