using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOS.Engineering.Challenge.Services
{
    public class RedisCacheService : IRedisCacheService
    {
   
        private readonly TimeSpan _absoluteExpiration;
        private readonly IDistributedCache _distributedCache;
        public RedisCacheService(IConfiguration configuration, IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
            var optionsSection = configuration.GetSection("Options");

            if (!int.TryParse(optionsSection.GetSection("RedisCacheMinutesTimeOut")?.Value, out int cacheTimeOut))
            {
                cacheTimeOut = 5;
            }
            _absoluteExpiration = TimeSpan.FromMinutes(cacheTimeOut);
        }

        public async void SetCache<T>(string cacheKey, T objeto)
        {
            DateTimeOffset expiration = DateTimeOffset.Now.Add(_absoluteExpiration);
            DistributedCacheEntryOptions options = new() { AbsoluteExpiration = expiration };
            await _distributedCache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(objeto), options);
        }

        public async Task<T> GetCache<T>(string cacheKey)
        {
            T? result = default;
            string? cachedMenber = await _distributedCache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedMenber))
            {
                result = JsonConvert.DeserializeObject<T?>(cachedMenber, new JsonSerializerSettings
                {
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                })!;
            }
            return result!;
        }

        public void DeleteCache(string cacheKey)
        {
            _distributedCache.Remove(cacheKey);
        }


    }
}
