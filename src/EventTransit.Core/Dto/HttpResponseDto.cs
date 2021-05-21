namespace EventTransit.Core.Dto
{
    public class HttpResponseDto
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public string Response { get; set; }
    }
}