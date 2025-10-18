using LigaLibre.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace LigaLibre.Infrastructure.Services;

public class RedisCacheService(IDistributedCache cache) : IRedisCacheService
{

    /// <summary>
    /// Obtener un objecto del cache Redis por su key 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<T?> GetAsync<T>(string key)
    {
        //Buscar en Redis por key (ej: clubs:1)
        var value = await cache.GetStringAsync(key);
        return value == null ? default : JsonSerializer.Deserialize<T>(value);
    }

    /// <summary>
    /// Guarda un objeto en Redis con TTL (tiempo de vida)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expiration"></param>
    /// <returns></returns>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var options = new DistributedCacheEntryOptions();
        if (expiration.HasValue)
            options.SetAbsoluteExpiration(expiration.Value);
        else
            options.SetSlidingExpiration(TimeSpan.FromMinutes(30));

        await cache.SetStringAsync(key, JsonSerializer.Serialize(value), options);
    }

    /// <summary>
    /// Elimina una key especifica del cache Redis
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task RemoveAsync(string key)
    {
        await cache.RemoveAsync(key);
    }

    /// <summary>
    /// Elimina multiples keys que coincidan con un patron
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public async Task RemovePatternAsync(string pattern)
    {
        if (pattern == "clubs:*")
        {
            await RemoveAsync("clubs:all");
        }
    }
}

