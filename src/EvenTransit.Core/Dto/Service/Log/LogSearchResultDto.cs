using System;
using System.Collections.Generic;
using EvenTransit.Core.Enums;

namespace EvenTransit.Core.Dto.Service.Log
{
    public class LogSearchResultDto
    {
        public List<LogFilterItemDto> Items { get; set; }
        public int TotalPages { get; set; }
    }
    
    public class LogFilterItemDto
    {
        public string Id { get; set; }
        public string EventName { get; set; }
        public string ServiceName { get; set; }
        public LogType LogType { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}