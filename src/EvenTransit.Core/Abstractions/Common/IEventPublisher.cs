using System.Threading.Tasks;
using EvenTransit.Core.Dto;

namespace EvenTransit.Core.Abstractions.Common
{
    public interface IEventPublisher
    {
        Task PublishAsync(string name, dynamic payload);
        void RegisterNewServiceAsync(NewServiceDto data);
    }
}