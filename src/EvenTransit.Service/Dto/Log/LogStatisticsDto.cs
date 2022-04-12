using System.Collections.Generic;

namespace EvenTransit.Service.Dto.Log;

public class LogStatisticsDto
{
    public List<string> Dates { get; set; }
    public List<long> SuccessCount { get; set; }
    public List<long> FailCount { get; set; }

    public LogStatisticsDto()
    {
        Dates = new List<string>();
        SuccessCount = new List<long>();
        FailCount = new List<long>();
    }
}
