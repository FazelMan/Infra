using System;
using System.Threading.Tasks;
using Infra.Shared.Ioc;
using Infra.Cache.Redis.Models;

namespace Infra.Cache.Redis.Services.Interfaces
{
    public interface IRedisService : ISingletonDependency
    { 
        void Set<T>(string key, T objectToCache, TimeSpan? expiry = null, int hits = 0, int version = 0, DateTime? created = null, DateTime? lastmodified = null) where T : class;
        void Set<T>(string key, T objectToCache, DateTimeOffset? expiry, int hits = 0, int version = 0, DateTime? created = null, DateTime? lastmodified = null) where T : class;
        CacheModel<T> Get<T>(string key) where T : class;
        void Delete(string key);
        void Refresh(string key);
        Task SetAsync<T>(string key, T objectToCache, TimeSpan? expiry = null, int hits = 0, int version = 0, DateTime? created = null, DateTime? lastmodified = null) where T : class;
        Task SetAsync<T>(string key, T objectToCache, DateTimeOffset? expiry, int hits = 0, int version = 0, DateTime? created = null, DateTime? lastmodified = null) where T : class;
        Task<T> GetAsync<T>(string key) where T : class;
        Task DeleteAsync(string key);
        Task RefreshAsync(string key);
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> build, TimeSpan? duration = default) where T : class;
    }

    public interface ICacheStatus
    {
        bool IsCacheEnabled { get; }
    }
}
