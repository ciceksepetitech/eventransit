namespace EvenTransit.UI.Models.Logs;

public class HttpResponseViewModel
{
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public string Response { get; set; }
}
