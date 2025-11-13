using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Core.Interfaces;
using System.Text.Json;

namespace SchoolManagementSystem.Infrastructure.Caching;

public class AdvancedCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<AdvancedCacheService> _logger;
    private readonly DistributedCacheEntryOptions _defaultOptions;

    public AdvancedCacheService(
        IDistributedCache cache,
        ILogger<AdvancedCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
        _defaultOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
            SlidingExpiration = TimeSpan.FromMinutes(10)
        };
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cachedData = await _cache.GetStringAsync(key, cancellationToken);

            if (string.IsNullOrEmpty(cachedData))
                return default;

            return JsonSerializer.Deserialize<T>(cachedData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving from cache: {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var options = expiration.HasValue
                ? new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration.Value }
                : _defaultOptions;

            var serializedData = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, serializedData, options, cancellationToken);

            _logger.LogDebug("Cached data with key: {Key}, expiration: {Expiration}", key, expiration ?? TimeSpan.FromMinutes(30));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache: {Key}", key);
        }
    }

    public async Task<T> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        // Try to get from cache
        var cachedValue = await GetAsync<T>(key, cancellationToken);
        if (cachedValue != null)
        {
            _logger.LogDebug("Cache hit for key: {Key}", key);
            return cachedValue;
        }

        _logger.LogDebug("Cache miss for key: {Key}, executing factory", key);

        // Execute factory to get fresh data
        var value = await factory();

        // Cache the result
        if (value != null)
        {
            await SetAsync(key, value, expiration, cancellationToken);
        }

        return value;
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
            _logger.LogDebug("Removed cache key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing from cache: {Key}", key);
        }
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        // Note: This requires Redis-specific implementation
        // For distributed cache, we'd need to track keys separately
        _logger.LogWarning("RemoveByPrefix not fully implemented for distributed cache: {Prefix}", prefix);
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cachedData = await _cache.GetStringAsync(key, cancellationToken);
            return !string.IsNullOrEmpty(cachedData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking cache existence: {Key}", key);
            return false;
        }
    }

    // Additional advanced caching methods

    public async Task<T?> GetWithRefreshAsync<T>(
        string key,
        Func<Task<T>> refreshFactory,
        TimeSpan refreshThreshold,
        TimeSpan expiration,
        CancellationToken cancellationToken = default)
    {
        var value = await GetAsync<T>(key, cancellationToken);

        if (value != null)
        {
            // Implement refresh-ahead caching
            // If cache is about to expire, refresh it in background
            _ = Task.Run(async () =>
            {
                var refreshedValue = await refreshFactory();
                if (refreshedValue != null)
                {
                    await SetAsync(key, refreshedValue, expiration, cancellationToken);
                }
            }, cancellationToken);
        }

        return value;
    }

    public async Task InvalidateRelatedCachesAsync(string entityType, int entityId, CancellationToken cancellationToken = default)
    {
        // Invalidate all caches related to an entity
        var patterns = new[]
        {
            $"{entityType}:{entityId}",
            $"{entityType}:List",
            $"{entityType}:Stats",
            $"Related:{entityType}:{entityId}"
        };

        foreach (var pattern in patterns)
        {
            await RemoveAsync(pattern, cancellationToken);
        }

        _logger.LogInformation("Invalidated caches for {EntityType}:{EntityId}", entityType, entityId);
    }
}
