using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOS.Engineering.Challenge.Services
{
    public interface IMemoryCacheService
    {
        public void SetCache<T>(string cacheKey, T objeto);
        public IEnumerable<T?> GetCache<T>(string cacheKey);
    }
}
