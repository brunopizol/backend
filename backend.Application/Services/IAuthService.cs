using backend.Application.DTOs;
using backend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.Application.Services
{
    public interface IAuthService
    {
        Task<AuthTokens> LoginAsync(LoginRequest req, string ip);
        Task<AuthTokens> RefreshAsync(RefreshRequest req, string ip);
        Task LogoutAsync(string refreshToken, string ip);
    }

}
