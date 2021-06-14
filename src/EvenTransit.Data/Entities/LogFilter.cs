using System.Collections.Generic;

namespace EvenTransit.Data.Entities
{
    public class LogFilter
    {
        public List<Logs> Items { get; set; }
        public int TotalPages { get; set; }
    }
}