using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class EmailVerify : IEmailVerify
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LinkGenerator _linkGenerator;
    private readonly RestaurantManagementDbContext _context;
    public EmailVerify(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor, RestaurantManagementDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _linkGenerator = linkGenerator;
        _context = context;
    }

    //tạo link xác thực email
    //TODO: Fix hard code link
    public string Create(EmailVerificationToken emailVerificationToken)
    {
        #region OldCode
        //string? link = _linkGenerator.GetUriByName(
        //        _httpContextAccessor.HttpContext!,
        //        "verify-email",
        //        new { emailVerificationToken.EmailVerificationTokenId });
        #endregion

        string? link = "https://localhost:7057/api/account/verify-email?token=" + emailVerificationToken.EmailVerificationTokenId;
        return link;
    }

    //Active Tài khoản
    public async Task<bool> Handle(Ulid tokenId)
    {
        //TODO: Refactor thành repository
        EmailVerificationToken token = await _context.EmailVerificationTokens
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.EmailVerificationTokenId == tokenId);

        if (token is null || token.ExpiredDate < DateTime.UtcNow || token.User.Status == "Activated")
        {
            return false;
        }

        token.User.Status = "Activated";
        _context.EmailVerificationTokens.Remove(token);
        await _context.SaveChangesAsync();
        return true;
    }
}
