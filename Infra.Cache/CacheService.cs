using System;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Infra.Shared.Helpers;
using Microsoft.Extensions.Configuration;

namespace Infra.Cache
{
    public class CacheService : ICacheService
    {
        private readonly ObjectCache cache;
        private readonly int duration;

        public CacheService()
        {
            cache = MemoryCache.Default;
            duration = Host.Config.GetValue<int>("Cache:CacheDurationSeconds");
        }

        public T Get<T>(string key) where T : class
        {
            return (T)this.cache.Get(key, null);
        }

        public T Get<T>(string key, Func<T> build) where T : class
        {
            return this.Get<T>(key, build, this.duration);
        }

        public T Get<T>(string key, Func<T> build, int duration) where T : class
        {
            return Get(key, build, null, duration);
        }

        public T Get<T>(string key, Func<T> build, bool recache) where T : class
        {
            return this.Get<T>(key, build, recache, this.duration);
        }

        public T Get<T>(string key, Func<T> build, bool recache, int duration) where T : class
        {
            CacheEntryRemovedCallback update = null;

            if (recache) update = new CacheEntryRemovedCallback((x) =>
            {
                this.Set(key, build(), duration);
            });

            return this.Get(key, build, update, duration);
        }

        public T Get<T>(string key, Func<T> build, CacheEntryRemovedCallback update) where T : class
        {
            return this.Get<T>(key, build, update, this.duration);
        }

        public T Get<T>(string key, Func<T> build, CacheEntryRemovedCallback update, int duration) where T : class
        {
            T data = this.Get<T>(key);

            if (data == null)
            {
                data = build();

                this.Set(key, data, update, duration);
            }

            return data;
        }

        public void Set<T>(string key, T value) where T : class
        {
            this.Set<T>(key, value, this.duration);
        }

        public void Set<T>(string key, T value, int duration) where T : class
        {
            this.Set(key, value, null, duration);
        }

        public void Set<T>(string key, T value, CacheEntryRemovedCallback update) where T : class
        {
            this.Set<T>(key, value, update, this.duration);
        }

        public void Set<T>(string key, T value, CacheEntryRemovedCallback update, int duration) where T : class
        {
            this.cache.Remove(key, null);

            CacheItem item = new CacheItem(key, value, null);

            CacheItemPolicy policy = new CacheItemPolicy()
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(duration),

                RemovedCallback = update
            };

            this.cache.Set(item, policy);
        }

        public T Remove<T>(string key) where T : class
        {
            return (T)this.cache.Remove(key, null);
        }
        public Task<T> GetAsync<T>(string key, Func<Task<T>> build) where T : class
        {
            return this.GetAsync<T>(key, build, this.duration);
        }

        public Task<T> GetAsync<T>(string key, Func<Task<T>> build, int duration) where T : class
        {
            return this.GetAsync(key, build, null, duration);
        }

        public Task<T> GetAsync<T>(string key, Func<Task<T>> build, bool recache) where T : class
        {
            return this.GetAsync<T>(key, build, recache, this.duration);
        }

        public Task<T> GetAsync<T>(string key, Func<Task<T>> build, bool recache, int duration) where T : class
        {
            CacheEntryRemovedCallback update = null;

            if (recache) update = new CacheEntryRemovedCallback(async (x) =>
            {
                T data = await build();

                this.Set<T>(key, data, duration);
            });

            return GetAsync(key, build, update, duration);
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> build, CacheEntryRemovedCallback update) where T : class
        {
            return await this.GetAsync<T>(key, build, update, this.duration);
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> build, CacheEntryRemovedCallback update, int duration) where T : class
        {
            T data = Get<T>(key);

            if (data == null)
            {
                data = await build();

                this.Set<T>(key, data, update, duration);
            }

            return data;
        }
    }
}
