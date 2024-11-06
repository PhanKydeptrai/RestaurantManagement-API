using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos
{
    public class EmailVerificationTokenRepository(RestaurantManagementDbContext context) : IEmailVerificationTokenRepository
    {
        public async Task CreateVerificationToken(EmailVerificationToken token)
        {
            await context.EmailVerificationTokens.AddAsync(token);
        }

        public async Task<EmailVerificationToken> GetVerificationTokenById(Ulid tokenId)
        {
            return await context.EmailVerificationTokens
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.EmailVerificationTokenId == tokenId);
        }

        public void RemoveVerificationToken(EmailVerificationToken token)
        {
            context.EmailVerificationTokens.Remove(token);
        }
    }
}
