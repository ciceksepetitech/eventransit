using System.Collections.Generic;
using EvenTransit.Core.Entities;

namespace EvenTransit.Core.Dto
{
    public class LogFilterDto
    {
        public List<Logs> Items { get; set; }
        public int TotalPages { get; set; }
    }
}