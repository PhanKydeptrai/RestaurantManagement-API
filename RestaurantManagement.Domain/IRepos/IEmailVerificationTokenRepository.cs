using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface IEmailVerificationTokenRepository
{
    Task<EmailVerificationToken>  GetVerificationTokenById(Ulid tokenId);
    Task CreateVerificationToken(EmailVerificationToken token);
    void RemoveVerificationToken(EmailVerificationToken token);
}
