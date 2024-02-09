using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace CarAuto.Caching;

public class RedisManager : IRedisManager
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<RedisManager> _logger;
    private readonly IConfiguration _configuration;
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisManager(
        IDistributedCache distributedCache,
        ILogger<RedisManager> logger,
        IConfiguration configuration,
        IConnectionMultiplexer connectionMultiplexer)
    {
        _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
    }

    public async Task<T> GetAsync<T>(string key, CancellationToken token = default)
    {
        _logger.LogTrace($"CacheManager.GetAsync called. IsConnected:{_connectionMultiplexer.IsConnected}");
        if (_connectionMultiplexer.IsConnected)
        {
            var result = await _distributedCache.GetAsync(key, token).ConfigureAwait(true);

            if (result != null)
            {
                return (T)ByteArrayToObject(result);
            }
        }

        return default(T);
    }

    public async Task RefreshAsync(string key, CancellationToken token = default)
    {
        _logger.LogTrace($"CacheManager.RefreshAsync called. IsConnected:{_connectionMultiplexer.IsConnected}");
        if (_connectionMultiplexer.IsConnected)
        {
            await _distributedCache.RefreshAsync(key, token).ConfigureAwait(true);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken token = default)
    {
        _logger.LogTrace($"CacheManager.RemoveAsync called. IsConnected:{_connectionMultiplexer.IsConnected}");
        if (_connectionMultiplexer.IsConnected)
        {
            await _distributedCache.RemoveAsync(key, token).ConfigureAwait(true);
        }
    }

    public async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options, CancellationToken token = default)
    {
        _logger.LogTrace($"CacheManager.SetAsync called with DistributedCacheEntryOptions. IsConnected:{_connectionMultiplexer.IsConnected}");
        if (_connectionMultiplexer.IsConnected)
        {
            await _distributedCache.SetAsync(key, ObjectToByteArray(value), options, token).ConfigureAwait(true);
        }
    }

    public async Task SetAsync<T>(string key, T value, CancellationToken token = default)
    {
        _logger.LogTrace($"CacheManager.SetAsync called. IsConnected:{_connectionMultiplexer.IsConnected}");
        if (_connectionMultiplexer.IsConnected)
        {
            await _distributedCache.SetAsync(key, ObjectToByteArray(value), GetDistributedCacheEntryOptions(), token).ConfigureAwait(true);
        }
    }

    public async Task RemoveKeys(string startsWith, bool isGeneric = false)
    {
        _logger.LogTrace($"CacheManager.RemoveKeys called. IsConnected:{_connectionMultiplexer.IsConnected}");

        if (_connectionMultiplexer.IsConnected)
        {
            if (string.IsNullOrWhiteSpace(startsWith))
            {
                throw new ArgumentNullException(nameof(startsWith));
            }

            string redisHost = _configuration["Redis:Host"] + ":" + _configuration["Redis:Port"];
            var cachedKeys = _connectionMultiplexer.GetServer(redisHost).Keys(pattern: startsWith + "*");
            if (cachedKeys != null)
            {
                foreach (var item in cachedKeys)
                {
                    await _distributedCache.RemoveAsync(item.ToString()).ConfigureAwait(false);
                }
            }
        }
        else
        {
            _logger.LogWarning($"Redis is not connected! startsWith=" + startsWith);
        }
    }

    public DistributedCacheEntryOptions GetDistributedCacheEntryOptions()
    {
        string absoluteExpirationDays = _configuration["Redis:AbsoluteExpirationDays"];
        string slidingExpirationInDays = _configuration["Redis:SlidingExpirationInDays"];

        return new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow =
            !string.IsNullOrWhiteSpace(absoluteExpirationDays)
            && int.TryParse(absoluteExpirationDays, out int absoluteExpirationDaysOut)
            ? TimeSpan.FromDays(absoluteExpirationDaysOut)
            : TimeSpan.FromDays(14),

            SlidingExpiration =
            !string.IsNullOrWhiteSpace(slidingExpirationInDays)
            && int.TryParse(slidingExpirationInDays, out int slidingExpirationInDaysOut)
            ? TimeSpan.FromDays(slidingExpirationInDaysOut)
            : TimeSpan.FromDays(3),
        };
    }

    private byte[] ObjectToByteArray(object obj)
    {
        if (obj.GetType() == typeof(byte[]))
        {
            return (byte[])obj;
        }

        BinaryFormatter bf = new BinaryFormatter();
        using (var ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    private object ByteArrayToObject(byte[] arrBytes)
    {
        using (var memStream = new MemoryStream())
        {
            var binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var obj = binForm.Deserialize(memStream);
            return obj;
        }
    }
}