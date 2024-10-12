using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos
{
    public class EmailVerificationTokenRepository : IEmailVerificationTokenRepository
    {
        private readonly RestaurantManagementDbContext _context;

        public EmailVerificationTokenRepository(RestaurantManagementDbContext context)
        {
            _context = context;
        }
        public async Task CreateVerificationToken(EmailVerificationToken token)
        {
            await _context.EmailVerificationTokens.AddAsync(token);
        }

        public async Task<EmailVerificationToken> GetVerificationTokenById(Ulid tokenId)
        {
            return await _context.EmailVerificationTokens
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.EmailVerificationTokenId == tokenId);
        }

        public void RemoveVerificationToken(EmailVerificationToken token)
        {
            _context.EmailVerificationTokens.Remove(token);
        }
    }
}
