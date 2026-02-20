using Infrastructure.Interfaces.Cache;

namespace Infrastructure.Services.Cache
{
    public static class CacheProvider
    {
        private static readonly Lazy<IMemoryCacheManager> _lazy =
            new Lazy<IMemoryCacheManager>(() => CreateInstance());

        private static IMemoryCacheManager CreateInstance()
        {            
            return new MemoryCacheManager();         
        }

        public static IMemoryCacheManager Instance => _lazy.Value;
    }
}
