using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.Application.DTOs
{

    public record LoginRequest(
        string Email,
        string Password,
        string? TotpCode,
        string DeviceId);

    public record AuthTokens(
        string AccessToken,
        string RefreshToken,
        DateTime ExpiresAt);

    public record RefreshRequest(
        string RefreshToken,
        string DeviceId);

}
