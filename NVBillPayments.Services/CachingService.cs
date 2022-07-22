using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using NVBillPayments.Core.Interfaces;
using NVBillPayments.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.Services
{
    public class CachingService : ICachingService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly string ENV_NAME = ConfigurationConstants.ENVIRONMENT;

        public CachingService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public Task Set(string key, object value, int timeInSeconds = 60)
        {
            return Task.Run(() =>
            {
                try
                {
                    _distributedCache.SetString(key+$"_{ENV_NAME}", JsonConvert.SerializeObject(value), new DistributedCacheEntryOptions
                    {
                        AbsoluteExpiration = new DateTimeOffset(DateTime.UtcNow.AddSeconds(timeInSeconds))
                    });
                }
                catch (Exception exp)
                {

                }
            });
        }

        public Task<T> Get<T>(string key)
        {
            return Task.Run(() =>
            {
                try
                {
                    var value = _distributedCache.GetString(key+$"_{ENV_NAME}");
                    if (!string.IsNullOrEmpty(value))
                        return JsonConvert.DeserializeObject<T>(value);
                    else
                        return default;
                }
                catch (Exception exp)
                {
                    return default;
                }
            });
        }
    }
}
