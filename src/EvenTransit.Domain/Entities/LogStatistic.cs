using System;

namespace EvenTransit.Domain.Entities
{
    public class LogStatistic :  BaseEntity
    {
        public long SuccessCount { get; set; }  
        
        public long FailCount { get; set; }
        
        public DateTime Date { get; set; }
    }
}