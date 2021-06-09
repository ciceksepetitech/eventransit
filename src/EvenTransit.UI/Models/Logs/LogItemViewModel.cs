using EvenTransit.Core.Enums;

namespace EvenTransit.UI.Models.Logs
{
    public class LogItemViewModel
    {
        public string EventName { get; set; }

        public string ServiceName { get; set; }

        public LogType Type { get; set; }

        public LogItemDetailViewModel Details { get; set; }
    }

    public class LogItemDetailViewModel
    {
        public HttpRequestViewModel Request { get; set; }

        public HttpResponseViewModel Response { get; set; }

        public string Message { get; set; }
    }
}