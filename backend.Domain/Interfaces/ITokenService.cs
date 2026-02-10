using backend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.Domain.Interfaces
{
    public interface ITokenService
    {
        string CreateAccessToken(User user, string deviceId);
        string GenerateRefreshToken();
    }
}
