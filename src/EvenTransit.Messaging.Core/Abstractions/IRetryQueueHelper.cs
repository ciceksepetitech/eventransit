using EvenTransit.Domain.Enums;
using EvenTransit.Messaging.Core.Domain;

namespace EvenTransit.Messaging.Core.Abstractions;

public interface IRetryQueueHelper
{
    List<RetryQueueInfo> RetryQueueInfo { get; }
    RetryTimes GetRetryQueue(long retryCount);
}