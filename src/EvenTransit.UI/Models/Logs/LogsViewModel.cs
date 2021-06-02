using System.Collections.Generic;

namespace EvenTransit.UI.Models.Logs
{
    public class LogsViewModel
    {
        public LogsViewModel()
        {
            LogList = new List<LogDetailViewModel>();
        }
        
        public List<LogDetailViewModel> LogList { get; set; }
    }
}