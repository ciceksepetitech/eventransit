using System.Collections.Generic;
using System.Threading.Tasks;
using EventTransit.Core.Entities;

namespace EventTransit.Core.Abstractions.Data
{
    public interface IMongoRepository<T>
    {
        Task<List<T>> GetEvents();
    }
}