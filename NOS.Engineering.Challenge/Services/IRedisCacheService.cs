using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOS.Engineering.Challenge.Services
{
    public interface IRedisCacheService
    {
        public void SetCache<T>(string cacheKey, T objeto);
        public Task<T> GetCache<T>(string cacheKey);
        public void DeleteCache(string cacheKey);
    }
}
