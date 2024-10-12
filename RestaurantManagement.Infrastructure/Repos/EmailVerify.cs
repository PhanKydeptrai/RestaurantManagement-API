using FluentEmail.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class EmailVerify : IEmailVerify
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LinkGenerator _linkGenerator;
    private readonly RestaurantManagementDbContext _context;
    private readonly IFluentEmail _fluentEmail;

    public EmailVerify(
        LinkGenerator linkGenerator, 
        IHttpContextAccessor httpContextAccessor, 
        RestaurantManagementDbContext context, 
        IFluentEmail fluentEmail)
    {
        _httpContextAccessor = httpContextAccessor;
        _linkGenerator = linkGenerator;
        _context = context;
        _fluentEmail = fluentEmail;
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

        string? link = "https://localhost:7057/api/account/verify-email?token=" + emailVerificationToken.EmailVerificationTokenId.ToString();
        return link;
    }
    public string CreateLinkForResetPass(EmailVerificationToken emailVerificationToken)
    {
        #region OldCode
        //string? link = _linkGenerator.GetUriByName(
        //        _httpContextAccessor.HttpContext!,
        //        "verify-email",
        //        new { emailVerificationToken.EmailVerificationTokenId });
        #endregion

        string? link = "https://localhost:7057/api/account/verify-reset-password?token=" + emailVerificationToken.EmailVerificationTokenId.ToString();
        return link;
    }

    public string CreateLinkForChangePass(EmailVerificationToken emailVerificationToken)
    {
        string? link = "https://localhost:7057/api/account/verify-change-password?token=" + emailVerificationToken.EmailVerificationTokenId.ToString();
        return link;
    }
    


    
}


