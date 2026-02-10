using backend.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.Application
{
    public class MemoryRateLimiter : IRateLimiter
    {
        private readonly IMemoryCache _cache;
        public MemoryRateLimiter(IMemoryCache cache) => _cache = cache;

        public Task<bool> IsAllowedAsync(string key, int limit, TimeSpan window)
        {
            var entry = _cache.GetOrCreate(key, e =>
            {
                e.AbsoluteExpirationRelativeToNow = window;
                return 0;
            });

            var count = (int)entry + 1;
            _cache.Set(key, count, window);
            return Task.FromResult(count <= limit);
        }
    }

}
