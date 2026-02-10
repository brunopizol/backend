using backend.Domain.Entities;
using backend.Domain.Interfaces;
using backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.Infrastructure.Repositories
{
    public class UserSecurityRepository : IUserSecurityRepository
    {
        private readonly ApplicationDbContext _ctx;

        public UserSecurityRepository(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        public async Task AddAsync(UserSecurity sec)
        {
            await _ctx.UserSecurities.AddAsync(sec);
            await _ctx.SaveChangesAsync();
        }
        public async Task<UserSecurity> GetAsync(Guid userId)
        {
            var security = await _ctx.Set<UserSecurity>()
                .AsTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (security == null)
            {
                security = new UserSecurity
                {
                    UserId = userId,
                    FailedLoginCount = 0,
                    LockoutEnd = null,
                    MfaEnabled = false,
                    TOTPSecret = null
                };

                _ctx.Set<UserSecurity>().Add(security);
                await _ctx.SaveChangesAsync();
            }

            return security;
        }

        public async Task UpdateAsync(UserSecurity sec)
        {
            var existing = await _ctx.Set<UserSecurity>()
                .FirstOrDefaultAsync(x => x.UserId == sec.UserId);

            if (existing == null)
                throw new InvalidOperationException(
                    $"UserSecurity not found for user {sec.UserId}");

            existing.FailedLoginCount = sec.FailedLoginCount;
            existing.LockoutEnd = sec.LockoutEnd;
            existing.MfaEnabled = sec.MfaEnabled;
            existing.TOTPSecret = sec.TOTPSecret;

            await _ctx.SaveChangesAsync();
        }
    }
}
