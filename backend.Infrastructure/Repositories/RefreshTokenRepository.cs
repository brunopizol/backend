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
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _ctx;

        public RefreshTokenRepository(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task AddAsync(RefreshToken token)
        {
            await _ctx.RefreshTokens.AddAsync(token);
            await _ctx.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
        {
            return await _ctx.RefreshTokens
                .AsTracking()
                .FirstOrDefaultAsync(x => x.TokenHash == tokenHash);
        }

        public async Task UpdateAsync(RefreshToken token)
        {
            var existing = await _ctx.RefreshTokens
                .FirstOrDefaultAsync(x => x.Id == token.Id);

            if (existing == null)
                throw new InvalidOperationException("RefreshToken not found");

            existing.RevokedAt = token.RevokedAt;
            existing.ReplacedByTokenHash = token.ReplacedByTokenHash;
            existing.ExpiresAt = token.ExpiresAt;
            existing.IpAddress = token.IpAddress;
            existing.DeviceId = token.DeviceId;

            await _ctx.SaveChangesAsync();
        }

        public async Task RevokeFamilyAsync(Guid userId, string deviceId, string reason)
        {
            var tokens = await _ctx.RefreshTokens
                .Where(x =>
                    x.UserId == userId &&
                    x.DeviceId == deviceId &&
                    x.RevokedAt == null)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.RevokedAt = DateTime.UtcNow;

                // se você quiser guardar motivo futuramente:
                // token.RevokeReason = reason;
            }

            await _ctx.SaveChangesAsync();
        }
    }
}
