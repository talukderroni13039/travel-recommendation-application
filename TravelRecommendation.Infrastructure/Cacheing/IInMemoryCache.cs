using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Infrastructure.Cacheing.InMemory
{
    using global::Backend.Application.Interface.Caching;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;

    namespace Backend.Infrastructure.Cacheing.InMemory
    {
        public class InMemoryCache : IInMemoryCache
        {
            private readonly IMemoryCache _memoryCache;
            private readonly TimeSpan _expiration;

            public InMemoryCache(IMemoryCache memoryCache, IConfiguration configuration)
            {
                _memoryCache = memoryCache;
                var expirationMinutes = Convert.ToInt16(configuration["CacheSettings:ExpirationMinutes"].ToString());
                _expiration = TimeSpan.FromMinutes(expirationMinutes);

            }

            public async Task<T> GetData<T>(string key)
            {
                var value = _memoryCache.Get(key);
                if (value != null)
                {
                    return await Task.FromResult(JsonSerializer.Deserialize<T>((string)value));
                }
                return default;
            }

            public async Task<bool> SetData<T>(string key, T value, DateTimeOffset expirationTime)
            {
                var serializedValue = JsonSerializer.Serialize(value);

                _memoryCache.Set(key, serializedValue, expirationTime);
                return await Task.FromResult(true);

            }
            public Task SetAsync<T>(string key, T value)
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException(nameof(key));

                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _expiration
                };
              //  var serializedValue = JsonSerializer.Serialize(value);
                _memoryCache.Set(key, value, options);
                return Task.CompletedTask;
            }
            public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory)
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException(nameof(key));

                if (factory == null)
                    throw new ArgumentNullException(nameof(factory));

                // Try to get from cache first
                if (_memoryCache.TryGetValue(key, out var cachedValue))
                    return (T)cachedValue;

                // If not in cache, execute factory and cache the result
                var newValue = await factory();
                await SetAsync(key, newValue);
                return newValue;
            }
            public async Task<bool> RemoveData(string key)
            {
                var value = _memoryCache.Get(key);
                if (value != null)
                {
                    _memoryCache.Remove(key);
                    return await Task.FromResult(true);
                }
                return await Task.FromResult(false);
            }
        }
    }

}
