using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Domain.IRepos;

public interface IEmailVerify
{
    string Create(EmailVerificationToken emailVerificationToken);
    string CreateLinkForResetPass(EmailVerificationToken emailVerificationToken);
}
