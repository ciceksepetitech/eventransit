using System.Collections.Generic;

namespace EvenTransit.Core.Dto
{
    public class LogDetailDto
    {
        public string Message { get; set; }
        public LogDetailRequestDto Request { get; set; }
        public LogDetailResponseDto Response { get; set; }
    }

    public class LogDetailRequestDto
    {
        public string Url { get; set; }
        public int Timeout { get; set; }
        public string Body { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }

    public class LogDetailResponseDto
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public string Response { get; set; }
    }
}