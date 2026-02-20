using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces.Cache
{
    public interface IMemoryCacheManager
    {
        void Add<T>(string key, T value, TimeSpan? expiration = null);
        T Get<T>(string key);
        T? GetOrCreate<T>(string key, Func<T?> factory, TimeSpan? expiration = null);
        Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T?>> factory, TimeSpan? expiration = null);
        void Remove(string key);
        bool Contains(string key);
    }
}
