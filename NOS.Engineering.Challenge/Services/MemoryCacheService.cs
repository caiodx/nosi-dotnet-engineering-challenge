using Microsoft.Extensions.Caching.Memory;
using NOS.Engineering.Challenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOS.Engineering.Challenge.Services
{
    public class MemoryCacheService : IMemoryCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan expiration = TimeSpan.FromDays(1); // Ajustar o tempo de acordo com a necessidade

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void SetCache<T>(string cacheKey, T objeto)
        {
            _cache.Set(cacheKey, objeto, expiration); 
        }

        public IEnumerable<T?> GetCache<T>(string cacheKey)
        {
            var cachedContent = _cache.Get<IEnumerable<T?>>(cacheKey);
            if (cachedContent == null) {
                _cache.TryGetValue(cacheKey, out cachedContent);
            }
            return cachedContent;
        }


    }
}
