using System.Collections.Generic;

namespace EvenTransit.Domain.Entities
{
    public class LogFilter
    {
        public List<Logs> Items { get; set; }
        public int TotalPages { get; set; }
    }
}