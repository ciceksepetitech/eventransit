using System;
using EvenTransit.Domain.Entities;

namespace EvenTransit.Domain.Abstractions;

public interface IServiceLockRepository
{
    void Insert(ServiceLock data);
    void Delete(string serviceName);
    ServiceLock GetByServiceName(string serviceName);
}
