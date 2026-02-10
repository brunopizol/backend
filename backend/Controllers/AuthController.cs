using backend.Application.DTOs;
using backend.Application.Services;
using backend.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController, Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) => _auth = auth;

        [HttpPost("login")]
        public async Task<ActionResult<AuthTokens>> Login(LoginRequest req)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var tokens = await _auth.LoginAsync(req, ip);
            return Ok(tokens);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthTokens>> Refresh(RefreshRequest req)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var tokens = await _auth.RefreshAsync(req, ip);
            return Ok(tokens);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            await _auth.LogoutAsync(refreshToken, ip);
            return NoContent();
        }
    }

}
