using System;
using System.Threading.Tasks;

namespace EvenTransit.Core.Abstractions.Common
{
    public interface ICacheService
    {
        Task SetAsync<T>(string key, T data, int expireMinutes = 60);
        Task<T> GetAsync<T>(string key, int expireMinutes, Func<Task<T>> acquire);
        Task DeleteAsync(string key);
    }
}