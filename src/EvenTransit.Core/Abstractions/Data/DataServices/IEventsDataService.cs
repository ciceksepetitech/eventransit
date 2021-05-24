using System.Collections.Generic;
using System.Threading.Tasks;

namespace EvenTransit.Core.Abstractions.Data.DataServices
{
    public interface IEventsDataService
    {
        Task<List<string>> GetQueueNamesByEventAsync(string eventName);
    }
}