using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Infra.Shared.Exceptions;
using Infra.Shared.Helpers;
using Infra.Cache.Redis.Models;
using Infra.Cache.Redis.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Infra.Cache.Redis.Services
{
    // public static partial class Extensions
    // {
    //     /// <summary>
    //     ///     An object extension method that serialize an object to binary.
    //     /// </summary>
    //     /// <typeparam name="T">Generic type parameter.</typeparam>
    //     /// <param name="this">The @this to act on.</param>
    //     /// <returns>A string.</returns>
    //     public static string SerializeBinary<T>(this T @this)
    //     {
    //         var binaryWrite = new BinaryFormatter();
    //
    //         using (var memoryStream = new MemoryStream())
    //         {
    //             binaryWrite.Serialize(memoryStream, @this);
    //             return Encoding.Default.GetString(memoryStream.ToArray());
    //         }
    //     }
    //
    //     /// <summary>
    //     ///     An object extension method that serialize an object to binary.
    //     /// </summary>
    //     /// <typeparam name="T">Generic type parameter.</typeparam>
    //     /// <param name="this">The @this to act on.</param>
    //     /// <param name="encoding">The encoding.</param>
    //     /// <returns>A string.</returns>
    //     public static string SerializeBinary<T>(this T @this, Encoding encoding)
    //     {
    //         var binaryWrite = new BinaryFormatter();
    //
    //         using (var memoryStream = new MemoryStream())
    //         {
    //             binaryWrite.Serialize(memoryStream, @this);
    //             return encoding.GetString(memoryStream.ToArray());
    //         }
    //     }
    // }

    public class RedisService : IRedisService, ICacheStatus
    {
        private readonly ILogger<RedisService> _logger;
        private static readonly object LockObject = new object();
        private RedisCache _RedisClient;
        private ConnectionMultiplexer _connectionMultiplexer;

        public RedisCache RedisClient
        {
            get
            {
                lock (LockObject)
                {
                    if (_RedisClient == null)
                    {
                        _RedisClient = new RedisCache(new RedisCacheOptions()
                        {
                            ConfigurationOptions = new ConfigurationOptions()
                            {
                                EndPoints =
                                {
                                    Host.Config["Redis:Endpoint"]
                                },
                                Ssl = false,
                                Password = Host.Config["Redis:Password"],
                                AbortOnConnectFail = false,
                                ConnectRetry = 2
                            }
                        });
                    }

                    return _RedisClient;
                }
            }
        }

        public ConnectionMultiplexer ConnectionMultiplexer
        {
            get
            {
                lock (LockObject)
                {
                    if (_connectionMultiplexer == null)
                    {
                        _connectionMultiplexer =
                            ConnectionMultiplexer.Connect(
                                new ConfigurationOptions()
                                {
                                    EndPoints =
                                    {
                                        Host.Config["Redis:Endpoint"]
                                    },
                                    Ssl = false,
                                    Password = Host.Config["Redis:Password"],
                                    AbortOnConnectFail = false,
                                    ConnectRetry = 5000
                                });

                        _connectionMultiplexer.GetServer("host").Keys();

                        return _connectionMultiplexer;
                    }

                    return _connectionMultiplexer;
                }
            }
        }

        public int CacheDurationSeconds
        {
            get
            {
                int isRedisCacheEnabled = 10;

                int.TryParse(Host.Config["Redis:CacheDurationSeconds"], out isRedisCacheEnabled);

                return isRedisCacheEnabled;
            }
        }

        public int CacheDurationSecondsRandom
        {
            get
            {
                int isRedisCacheEnabled = 5;

                int.TryParse(Host.Config["Redis:CacheDurationSecondsRandom"], out isRedisCacheEnabled);

                return isRedisCacheEnabled;
            }
        }

        public int MaxCacheDurationSeconds
        {
            get
            {
                int isRedisCacheEnabled = 60;

                int.TryParse(Host.Config["Redis:MaxCacheDurationSeconds"], out isRedisCacheEnabled);

                return isRedisCacheEnabled;
            }
        }

        public RedisService(ILogger<RedisService> logger)
        {
            _logger = logger;
        }


        #region [[Private Methods]]

        private T RGet<T>(string key)
        {
            var data = RedisClient.Get(key);

            if (data == null)
            {
                return default(T);
            }
            else
            {
                var bytesAsString = Encoding.UTF8.GetString(data);
                return JsonConvert.DeserializeObject<T>(bytesAsString);
            }
        }

        private void RSet<T>(string key, T val, TimeSpan? expire = null)
        {
            var options = new DistributedCacheEntryOptions();

            if (expire.HasValue)
            {
                options.SetAbsoluteExpiration(expire.Value);
            }
            else
            {
                options.SetAbsoluteExpiration(TimeSpan.FromSeconds(10));
            }

            RedisClient.Set(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(val)), options);
        }

        private void RSet<T>(string key, T val, DateTimeOffset? expire = null)
        {
            var options = new DistributedCacheEntryOptions();

            if (expire.HasValue)
            {
                options.SetAbsoluteExpiration(expire.Value);
            }
            else
            {
                options.SetAbsoluteExpiration(new DateTimeOffset(DateTime.UtcNow,
                    new TimeSpan(0, 0, CacheDurationSeconds)));
            }

            RedisClient.Set(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(val)), options);
        }

        private void RDel(string key)
        {
            RedisClient.Remove(key);
        }

        private void RFlush(string key)
        {
            RedisClient.Refresh(key);
        }

        private TimeSpan GenerateCacheExpiry(int hits = 0)
        {
            int totalSecond = CacheDurationSeconds + new Random().Next(0, CacheDurationSecondsRandom) + hits;

            return TimeSpan.FromSeconds(totalSecond > MaxCacheDurationSeconds ? MaxCacheDurationSeconds : totalSecond);
        }

        private void UpdateHitCount<T>(string key, CacheModel<T> result) where T : class
        {
            CacheModel<T> model = new CacheModel<T>(result.Data, ++result.Hits, result.Version, result.Created);

            RSet(key, model, GenerateCacheExpiry(result.Hits));
        }

        private async Task<T> RGetAsync<T>(string key)
        {
            try
            {
                var data = await RedisClient.GetAsync(key);

                if (data == null)
                {
                    return default(T);
                }
                else
                {
                    var bytesAsString = Encoding.UTF8.GetString(data);
                    return JsonConvert.DeserializeObject<T>(bytesAsString);
                }
            }
            catch (RedisConnectionException e)
            {
                _logger.LogError(new Exception(e.Message), e.Message);
                return default(T);
            }
        }

        private async Task RSetAsync<T>(string key, T val, TimeSpan? expire = null)
        {
            var options = new DistributedCacheEntryOptions();

            if (expire.HasValue)
            {
                options.SetAbsoluteExpiration(expire.Value);
            }
            else
            {
                options.SetAbsoluteExpiration(TimeSpan.FromSeconds(10));
            }

            await RedisClient.SetAsync(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(val)), options);
        }

        private async Task RSetAsync<T>(string key, T val, DateTimeOffset? expire = null)
        {
            var options = new DistributedCacheEntryOptions();

            if (expire.HasValue)
            {
                options.SetAbsoluteExpiration(expire.Value);
            }
            else
            {
                options.SetAbsoluteExpiration(new DateTimeOffset(DateTime.UtcNow,
                    new TimeSpan(0, 0, CacheDurationSeconds)));
            }

            await RedisClient.SetAsync(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(val)), options);
        }

        private async Task RDelAsync(string key)
        {
            await RedisClient.RemoveAsync(key);
        }

        private async Task RFlushAsync(string key)
        {
            await RedisClient.RefreshAsync(key);
        }

        private async Task UpdateHitCountAsync<T>(string key, CacheModel<T> result) where T : class
        {
            CacheModel<T> model = new CacheModel<T>(result.Data, ++result.Hits, result.Version, result.Created);

            await RSetAsync(key, model, GenerateCacheExpiry(result.Hits));
        }

        #endregion

        public void Set<T>(string key, T objectToCache, TimeSpan? expiry = null, int hits = 0, int version = 0,
            DateTime? created = null, DateTime? lastmodified = null) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key in Redis Set Operation");
            }

            CacheModel<T> model = new CacheModel<T>(objectToCache, hits, version, created, lastmodified);

            if (IsCacheEnabled)
            {
                try
                {
                    RSet(key, model, expiry ?? GenerateCacheExpiry());
                }
                catch (Exception e)
                {
                    throw new InternalServerException(
                        $"Unsuccessful Redis Set Operation by key [{key}]. Descrption: {e.Message}");
                }
            }
        }

        public void Set<T>(string key, T objectToCache, DateTimeOffset? expiry, int hits = 0, int version = 0,
            DateTime? created = null, DateTime? lastmodified = null) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key in Redis Set Operation");
            }

            CacheModel<T> model = new CacheModel<T>(objectToCache, hits, version, created, lastmodified);

            if (IsCacheEnabled)
            {
                try
                {
                    RSet(key, model, expiry);
                }
                catch (Exception e)
                {
                    throw new InternalServerException(
                        $"Unsuccessful Redis Set Operation by key [{key}]. Descrption: {e.Message}");
                }
            }
        }

        public CacheModel<T> Get<T>(string key) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key in Redis Get Operation");
            }

            if (IsCacheEnabled)
            {
                try
                {
                    var result = RGet<CacheModel<T>>(key);

                    if (result != null)
                    {
                        UpdateHitCount(key, result);
                    }

                    return result;
                }
                catch (Exception e)
                {
                    throw new InternalServerException(
                        $"Unsuccessful Redis Get Operation by key [{key}]. Descrption: {e.Message}");
                }
            }

            return null;
        }

        public void Delete(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key in Redis Delete Operation");
            }

            if (IsCacheEnabled)
            {
                try
                {
                    RDel(key);
                }
                catch (Exception e)
                {
                    throw new InternalServerException(
                        $"Unsuccessful Redis Delete Operation by key [{key}]. Descrption: {e.Message}");
                }
            }
        }

        public void Refresh(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key in Redis Refresh Operation");
            }

            if (IsCacheEnabled)
            {
                try
                {
                    RFlush(key);
                }
                catch (Exception e)
                {
                    throw new InternalServerException(
                        $"Unsuccessful Redis Refresh Operation by key [{key}]. Descrption: {e.Message}");
                }
            }
        }

        public async Task SetAsync<T>(string key, T objectToCache, TimeSpan? expiry = null, int hits = 0,
            int version = 0, DateTime? created = null, DateTime? lastmodified = null) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key in Redis Set Operation");
            }

            CacheModel<T> model = new CacheModel<T>(objectToCache, hits, version, created, lastmodified);

            if (IsCacheEnabled)
            {
                try
                {
                    await RSetAsync(key, model, expiry ?? GenerateCacheExpiry());
                }
                catch (Exception e)
                {
                    _logger.LogError($"Unsuccessful Redis Set Operation by key [{key}]. Descrption: {e.Message}");
                }
            }
        }

        public async Task SetAsync<T>(string key, T objectToCache, DateTimeOffset? expiry, int hits = 0,
            int version = 0, DateTime? created = null, DateTime? lastmodified = null) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key in Redis Set Operation");
            }

            CacheModel<T> model = new CacheModel<T>(objectToCache, hits, version, created, lastmodified);

            if (IsCacheEnabled)
            {
                try
                {
                    await RSetAsync(key, model, expiry);
                }
                catch (Exception e)
                {
                    throw new InternalServerException(
                        $"Unsuccessful Redis Set Operation by key [{key}]. Descrption: {e.Message}");
                }
            }
        }

        public async Task<T> GetAsync<T>(string key) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key in Redis Get Operation");
            }

            if (IsCacheEnabled)
            {
                try
                {
                    var result = await RGetAsync<CacheModel<T>>(key);

                    // if (result != null)
                    // {
                    //     await UpdateHitCountAsync(key, result);
                    // }

                    return result?.Data;
                }
                catch (Exception e)
                {
                    throw new InternalServerException(
                        $"Unsuccessful Redis Get Operation by key [{key}]. Descrption: {e.Message}");
                }
            }

            return null;
        }

        public async Task DeleteAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key in Redis Delete Operation");
            }

            if (IsCacheEnabled)
            {
                try
                {
                    await RDelAsync(key);
                }
                catch (Exception e)
                {
                    throw new InternalServerException(
                        $"Unsuccessful Redis Delete Operation by key [{key}]. Descrption: {e.Message}");
                }
            }
        }

        public async Task RefreshAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key in Redis Refresh Operation");
            }

            if (IsCacheEnabled)
            {
                try
                {
                    await RFlushAsync(key);
                }
                catch (Exception e)
                {
                    throw new InternalServerException(
                        $"Unsuccessful Redis Refresh Operation by key [{key}]. Descrption: {e.Message}");
                }
            }
        }

        public bool IsCacheEnabled
        {
            get
            {
                bool isRedisCacheEnabled = false;

                bool.TryParse(Host.Config["Redis:IsRedisCacheEnabled"], out isRedisCacheEnabled);

                return isRedisCacheEnabled;
            }
        }

        //TODO should be remove
        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> build, TimeSpan? duration = default)
            where T : class
        {
            try
            {
                var cacheValue = await this.GetAsync<T>(key);

                if (cacheValue != default)
                    return cacheValue;

                cacheValue = await build();

                await this.SetAsync(key, cacheValue, duration);

                return cacheValue;
            }
            catch (Exception exc)
            {
                // Todo: Log exc
                return await build();
            }
        }
    }
}