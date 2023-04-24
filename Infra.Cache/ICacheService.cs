using System;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Infra.Shared.Ioc;

namespace Infra.Cache
{
    public interface ICacheService : ISingletonDependency
    {
        T Get<T>(string key) where T : class;
        T Get<T>(string key, Func<T> build) where T : class;
        T Get<T>(string key, Func<T> build, int duration) where T : class;
        T Get<T>(string key, Func<T> build, bool recache) where T : class;
        T Get<T>(string key, Func<T> build, bool recache, int duration) where T : class;
        T Get<T>(string key, Func<T> build, CacheEntryRemovedCallback update) where T : class;
        T Get<T>(string key, Func<T> build, CacheEntryRemovedCallback update, int duration) where T : class;
        void Set<T>(string key, T value) where T : class;
        void Set<T>(string key, T value, int duration) where T : class;
        void Set<T>(string key, T value, CacheEntryRemovedCallback update) where T : class;
        void Set<T>(string key, T value, CacheEntryRemovedCallback update, int duration) where T : class;
        T Remove<T>(string key) where T : class;
        Task<T> GetAsync<T>(string key, Func<Task<T>> build) where T : class;
        Task<T> GetAsync<T>(string key, Func<Task<T>> build, int duration) where T : class;
        Task<T> GetAsync<T>(string key, Func<Task<T>> build, bool recache) where T : class;
        Task<T> GetAsync<T>(string key, Func<Task<T>> build, bool recache, int duration) where T : class;
        Task<T> GetAsync<T>(string key, Func<Task<T>> build, CacheEntryRemovedCallback update) where T : class;
        Task<T> GetAsync<T>(string key, Func<Task<T>> build, CacheEntryRemovedCallback update, int duration) where T : class;
    }
}