using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.Domain.Interfaces
{
    public interface IRateLimiter
    {
        Task<bool> IsAllowedAsync(string key, int limit, TimeSpan window);
    }
}
