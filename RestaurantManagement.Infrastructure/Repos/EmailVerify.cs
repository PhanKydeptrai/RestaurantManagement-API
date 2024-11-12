using FluentEmail.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class EmailVerify(
    LinkGenerator linkGenerator,
    IHttpContextAccessor httpContextAccessor,
    RestaurantManagementDbContext context,
    IFluentEmail fluentEmail) : IEmailVerify
{

    public string Create(EmailVerificationToken emailVerificationToken)
    {
        
        string? link = linkGenerator.GetUriByName(
               httpContextAccessor.HttpContext!,
               "verify-email",
               new { token = emailVerificationToken.EmailVerificationTokenId });
        
        return link;
    }
    public string CreateLinkForResetPass(EmailVerificationToken emailVerificationToken)
    {
        string? link = linkGenerator.GetUriByName(
               httpContextAccessor.HttpContext!,
               "verify-reset-password",
               new { token = emailVerificationToken.EmailVerificationTokenId });

        return link;
    }

    public string CreateLinkForChangePass(EmailVerificationToken emailVerificationToken)
    {

        string? link = linkGenerator.GetUriByName(
               httpContextAccessor.HttpContext!,
               "verify-change-password",
               new { token = emailVerificationToken.EmailVerificationTokenId });

        return link;
    }


    public string CreateLinkForDeleteCustomerAccount(EmailVerificationToken emailVerificationToken)
    {
        string? link = linkGenerator.GetUriByName(
               httpContextAccessor.HttpContext!,
               "confirm-delete-account",
               new { token = emailVerificationToken.EmailVerificationTokenId });

        return link;
    }

}


