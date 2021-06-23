using System;

namespace EvenTransit.Service.Locker
{
    public interface IDistributedLocker
    {
        bool Acquire(string serviceName);
        void Release();
    }
}