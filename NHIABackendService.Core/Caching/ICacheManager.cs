using System;
using System.Threading.Tasks;

namespace NHIABackendService.Core.Caching
{
    public interface ICacheManager : IDisposable
    {
        T Get<T>(string key);
        void Set(string key, object data, int cacheTime);
        bool IsSet(string key);
        void Remove(string key);
        Task<T> AddOrGetAsync<T>(string key, Func<Task<T>> fetch, int cacheTime);
    }
}
