using System;

namespace EvenTransit.Domain.Entities
{
    public class ServiceLock : BaseEntity
    {
        public string ServiceName { get; set; }
        public DateTime LockStartDate { get; set; }
    }
}