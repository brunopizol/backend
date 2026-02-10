using backend.Application.DTOs;
using backend.Domain.Entities;
using backend.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace backend.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IUserSecurityRepository _secRepo;
        private readonly IRefreshTokenRepository _refreshRepo;
        private readonly IPasswordHasher _hasher;
        private readonly ITokenService _tokens;
        private readonly IRateLimiter _rate;
        private readonly IAuditService _audit;
        private readonly JwtOptions _jwt;

        public AuthService(
            IUserRepository users,
            IUserSecurityRepository secRepo,
            IRefreshTokenRepository refreshRepo,
            IPasswordHasher hasher,
            ITokenService tokens,
            IRateLimiter rate,
            IAuditService audit,
            IOptions<JwtOptions> jwt)
        {
            _users = users; _secRepo = secRepo; _refreshRepo = refreshRepo;
            _hasher = hasher; _tokens = tokens; _rate = rate; _audit = audit;
            _jwt = jwt.Value;
        }

        public async Task<AuthTokens> LoginAsync(LoginRequest req, string ip)
        {
            // Rate limit por IP + email
            if (!await _rate.IsAllowedAsync($"login:ip:{ip}", 20, TimeSpan.FromMinutes(1)) ||
                !await _rate.IsAllowedAsync($"login:user:{req.Email}", 10, TimeSpan.FromMinutes(1)))
                throw new UnauthorizedAccessException("Too many attempts");

            var user = await _users.GetByEmailAsync(req.Email)
                       ?? throw new UnauthorizedAccessException("Invalid credentials");

            var sec = await _secRepo.GetAsync(user.Id);

            if (sec.LockoutEnd is DateTime until && until > DateTime.UtcNow)
                throw new UnauthorizedAccessException("Account locked");

            // senha
            var valid = _hasher.Verify(req.Password, user.Password, out var needsRehash);
            if (!valid)
            {
                await RegisterFailure(sec, ip, req.DeviceId, user.Id);
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            // MFA
            if (sec.MfaEnabled)
            {
                if (string.IsNullOrWhiteSpace(req.TotpCode) ||
                    !TotpHelper.Verify(sec.TOTPSecret!, req.TotpCode))
                    throw new UnauthorizedAccessException("MFA required/invalid");
            }

            // sucesso → reset falhas
            sec.FailedLoginCount = 0;
            sec.LockoutEnd = null;
            await _secRepo.UpdateAsync(sec);

            // auto rehash
            if (needsRehash)
            {
                user.ChangePassword(_hasher.Hash(req.Password));
                await _users.UpdateAsync(user);
            }

            // tokens
            var access = _tokens.CreateAccessToken(user, req.DeviceId);
            var refreshRaw = _tokens.GenerateRefreshToken();
            var refreshHash = Convert.ToBase64String(
                SHA256.HashData(Encoding.UTF8.GetBytes(refreshRaw)));

            var refresh = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshHash,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwt.RefreshDays),
                DeviceId = req.DeviceId,
                IpAddress = ip
            };
            await _refreshRepo.AddAsync(refresh);

            await _audit.LogAsync(new AuditLog
            {
                UserId = user.Id,
                Event = "login.success",
                Ip = ip,
                DeviceId = req.DeviceId
            });

            return new AuthTokens(access, refreshRaw,
                DateTime.UtcNow.AddMinutes(_jwt.AccessMinutes));
        }

        public async Task<AuthTokens> RefreshAsync(RefreshRequest req, string ip)
        {
            var hash = Convert.ToBase64String(
                SHA256.HashData(Encoding.UTF8.GetBytes(req.RefreshToken)));

            var token = await _refreshRepo.GetByTokenHashAsync(hash)
                        ?? throw new UnauthorizedAccessException("Invalid refresh");

            if (!token.IsActive || token.DeviceId != req.DeviceId)
                throw new UnauthorizedAccessException("Invalid refresh");

            var user = await _users.GetByIdAsync(token.UserId)
                       ?? throw new UnauthorizedAccessException();

            // ROTATION: revoga o atual e emite novo
            token.RevokedAt = DateTime.UtcNow;

            var newRaw = _tokens.GenerateRefreshToken();
            var newHash = Convert.ToBase64String(
                SHA256.HashData(Encoding.UTF8.GetBytes(newRaw)));

            token.ReplacedByTokenHash = newHash;
            await _refreshRepo.UpdateAsync(token);

            var newToken = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = newHash,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwt.RefreshDays),
                DeviceId = req.DeviceId,
                IpAddress = ip
            };
            await _refreshRepo.AddAsync(newToken);

            var access = _tokens.CreateAccessToken(user, req.DeviceId);

            await _audit.LogAsync(new AuditLog
            {
                UserId = user.Id,
                Event = "token.refresh",
                Ip = ip,
                DeviceId = req.DeviceId
            });

            return new AuthTokens(access, newRaw,
                DateTime.UtcNow.AddMinutes(_jwt.AccessMinutes));
        }

        public async Task LogoutAsync(string refreshToken, string ip)
        {
            var hash = Convert.ToBase64String(
                SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));

            var token = await _refreshRepo.GetByTokenHashAsync(hash);
            if (token == null) return;

            token.RevokedAt = DateTime.UtcNow;
            await _refreshRepo.UpdateAsync(token);

            await _audit.LogAsync(new AuditLog
            {
                UserId = token.UserId,
                Event = "logout",
                Ip = ip,
                DeviceId = token.DeviceId
            });
        }

        private async Task RegisterFailure(UserSecurity sec, string ip, string deviceId, Guid userId)
        {
            sec.FailedLoginCount++;
            if (sec.FailedLoginCount >= 5)
            {
                var minutes = Math.Min(60, sec.FailedLoginCount * 2); // lock progressivo
                sec.LockoutEnd = DateTime.UtcNow.AddMinutes(minutes);
            }
            await _secRepo.UpdateAsync(sec);

            await _audit.LogAsync(new AuditLog
            {
                UserId = userId,
                Event = "login.failure",
                Ip = ip,
                DeviceId = deviceId
            });
        }
    }


}
