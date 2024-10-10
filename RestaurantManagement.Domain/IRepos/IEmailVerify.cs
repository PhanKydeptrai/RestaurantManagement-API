using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface IEmailVerify
{
    Task<bool> Handle(Ulid tokenId);
    string Create(EmailVerificationToken emailVerificationToken);
}
