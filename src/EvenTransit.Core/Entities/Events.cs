using System.Collections.Generic;

namespace EvenTransit.Core.Entities
{
    public class Events : BaseEntity
    {
        public string Name { get; set; }
        public List<Service> Services { get; set; }
    }
}