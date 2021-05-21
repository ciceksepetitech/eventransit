using System;
using System.Linq;
using EventTransit.Core.Abstractions.QueueProcess;
using EventTransit.Core.Enums;
using EventTransit.Messaging.Core.Domain.QueueProcess;
using Microsoft.Extensions.DependencyInjection;

namespace EventTransit.Messaging.Core.Domain.Common
{
    public class QueueProcessResolver : IQueueProcessResolver
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public QueueProcessResolver(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public IQueueProcess Resolve(QueueProcessType processType)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var services = scope.ServiceProvider.GetServices(typeof(IQueueProcess));
            return processType switch
            {
                QueueProcessType.HttpRequest => (IQueueProcess) services.FirstOrDefault(m => m.GetType() == typeof(HttpQueueProcess)),
                _ => null
            };
        }
    }
}