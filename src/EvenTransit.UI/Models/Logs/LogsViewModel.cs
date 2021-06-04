using System.Collections.Generic;

namespace EvenTransit.UI.Models.Logs
{
    public class LogsViewModel
    {
        public LogList LogList { get; set; }
    }

    public class LogList
    {
        public LogList()
        {
            Items = new List<LogSearchResultViewModel>();
        }
        
        public List<LogSearchResultViewModel> Items { get; set; }
        public int TotalPages { get; set; }
    }
}