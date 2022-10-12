using System;
using System.Collections.Generic;
using EvenTransit.Domain.Enums;

namespace EvenTransit.Messaging.Core.Domain;

public class RetryQueueHelper
{
    public List<RetryQueueInfo> RetryQueueInfo { get; }

    public RetryQueueHelper()
    {
        RetryQueueInfo = new List<RetryQueueInfo>();
        GetEnumAttributes();
    }

    public RetryTimes GetRetryQueue(long retryCount)
    {
        return retryCount switch
        {
            >= 0 and <= 2 => RetryTimes.Five,
            > 2 and <= 4 => RetryTimes.Thirty,
            _ => RetryTimes.Sixty
        };
    }

    private void GetEnumAttributes()
    {
        var enumType = typeof(RetryTimes);
        foreach (var val in Enum.GetValues(enumType))
            RetryQueueInfo.Add(new RetryQueueInfo
            {
                RetryTime = (RetryTimes)val,
                TTL = Convert.ToInt32(val) * 1000,
                QueueSuffix = Enum.GetName(typeof(RetryTimes), val)
            });
    }
}

public class RetryQueueInfo
{
    public RetryTimes RetryTime { get; set; }
    public int TTL { get; set; }
    public string QueueSuffix { get; set; }
}
