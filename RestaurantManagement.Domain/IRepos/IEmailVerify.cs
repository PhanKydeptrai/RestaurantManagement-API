using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface IEmailVerify
{
    string Create(EmailVerificationToken emailVerificationToken);
    string CreateLinkForResetPass(EmailVerificationToken emailVerificationToken);
    string CreateLinkForChangePass(EmailVerificationToken emailVerificationToken);
    string CreateLinkForDeleteCustomerAccount(EmailVerificationToken emailVerificationToken);
}
