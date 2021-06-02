using EvenTransit.Core.Enums;

namespace EvenTransit.UI.Models.Logs
{
    public class LogDetailViewModel
    {
        public string EventName { get; set; }

        public string ServiceName { get; set; }

        public LogType Type { get; set; }

        public HttpRequestViewModel Request { get; set; }

        public HttpResponseViewModel Response { get; set; }

        public string Message { get; set; }
    }
}