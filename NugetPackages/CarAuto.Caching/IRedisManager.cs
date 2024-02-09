using Microsoft.Extensions.Caching.Distributed;
using System.Threading;
using System.Threading.Tasks;

namespace CarAuto.Caching;

public interface IRedisManager
{
    Task<T> GetAsync<T>(string key, CancellationToken token = default);

    Task RefreshAsync(string key, CancellationToken token = default);

    Task RemoveAsync(string key, CancellationToken token = default);

    Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options, CancellationToken token = default);

    Task SetAsync<T>(string key, T value, CancellationToken token = default);

    Task RemoveKeys(string startsWith, bool isGeneric = false);

    DistributedCacheEntryOptions GetDistributedCacheEntryOptions();
}