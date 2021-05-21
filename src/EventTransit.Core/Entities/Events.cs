using System.Collections.Generic;

namespace EventTransit.Core.Entities
{
    public class Events : BaseEntity
    {
        public string Name { get; set; }
        public List<Service> Services { get; set; }
    }
}