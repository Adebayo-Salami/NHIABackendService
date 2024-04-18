using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace NHIABackendService.Core.Caching
{
    public class MemoryCacheManager : ICacheManager
    {
        public MemoryCacheManager()
        {
            Cache = new MemoryCache(new MemoryCacheOptions());
        }

        public IMemoryCache Cache { get; }

        public virtual T Get<T>(string key)
        {
            return (T)Cache.Get(key);
        }

        public virtual void Set(string key, object data, int cacheTime)
        {
            if (data == null)
                return;

            Cache.Set(key, data, DateTime.Now + TimeSpan.FromMinutes(cacheTime));
        }

        public virtual bool IsSet(string key)
        {
            return Cache.Get(key) != null;
        }

        public virtual void Remove(string key)
        {
            Cache.Remove(key);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Cleanup
        }

        public async Task<T> AddOrGetAsync<T>(string key, Func<Task<T>> fetch, int cacheTime)
        {
            if (IsSet(key))
            {
                return Get<T>(key);
            }

            var result = await fetch.Invoke();
            Cache.Set(key, result);
            return result;
        }
    }
}
