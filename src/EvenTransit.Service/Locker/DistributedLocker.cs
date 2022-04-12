using EvenTransit.Domain.Abstractions;
using EvenTransit.Domain.Entities;

namespace EvenTransit.Service.Locker;

public class DistributedLocker : IDistributedLocker
{
    private string ServiceName { get; set; }

    private readonly IServiceLockRepository _serviceLockRepository;

    public DistributedLocker(IServiceLockRepository serviceLockRepository)
    {
        _serviceLockRepository = serviceLockRepository;
    }

    public bool Acquire(string serviceName)
    {
        ServiceName = serviceName;

        var lockInfo = _serviceLockRepository.GetByServiceName(serviceName);

        if (lockInfo != null) return false;

        _serviceLockRepository.Insert(new ServiceLock { ServiceName = serviceName });

        return true;
    }

    public void Release()
    {
        var lockInfo = _serviceLockRepository.GetByServiceName(ServiceName);

        if (lockInfo != null) _serviceLockRepository.Delete(ServiceName);
    }
}
