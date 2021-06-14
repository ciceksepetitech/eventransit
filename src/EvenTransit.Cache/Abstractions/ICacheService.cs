using System;
using System.Threading.Tasks;
using EvenTransit.Core.Enums;

namespace EvenTransit.Cache.Abstractions
{
    public interface ICacheService
    {
        Task SetAsync<T>(string key, T data, ExpireTimes expireTimes = ExpireTimes.OneHour);
        Task<T> GetAsync<T>(string key, ExpireTimes expireTimes, Func<Task<T>> acquire);
        Task DeleteAsync(string key);
    }
}