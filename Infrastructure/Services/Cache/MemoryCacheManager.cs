using Infrastructure.Interfaces.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services.Cache
{
    public sealed class MemoryCacheManager : IMemoryCacheManager
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheManager()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public void Add<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new MemoryCacheEntryOptions();
            if (expiration.HasValue)
                options.AbsoluteExpirationRelativeToNow = expiration;

            _cache.Set(key, value, options);
        }

        public T Get<T>(string key)
        {
            return _cache.TryGetValue(key, out T value) ? value : default;
        }

        public T? GetOrCreate<T>(string key, Func<T?> factory, TimeSpan? expiration = null)
        {
            return _cache.GetOrCreate(key, entry =>
            {
                if (expiration.HasValue)
                    entry.AbsoluteExpirationRelativeToNow = expiration;

                return factory();
            });
        }

        public Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T?>> factory, TimeSpan? expiration = null)
        {
            return _cache.GetOrCreateAsync(key, async entry =>
            {
                if (expiration.HasValue)
                    entry.AbsoluteExpirationRelativeToNow = expiration;

                return await factory();
            });
        }

        public void Remove(string key) => _cache.Remove(key);

        public bool Contains(string key) => _cache.TryGetValue(key, out _);

    }
}
