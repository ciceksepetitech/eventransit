using System.Text;
using System.Text.Json;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.Core.Dto;
using System.Net;

namespace EvenTransit.Messaging.Core.Domain;

public class HttpRequestSender : IHttpRequestSender
{
    private const int _defaultTimeout = 20;
    private const int _maxTimeout = 60;
    private readonly IHttpClientFactory _clientFactory;

    public HttpRequestSender(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<HttpResponseDto> SendAsync(HttpRequestDto request)
    {
        var requestMessage = new HttpRequestMessage();
        var httpClient = _clientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(request.Url);

        httpClient.Timeout = TimeSpan.FromSeconds(_defaultTimeout);

        if (request.Timeout > 0)
            httpClient.Timeout = TimeSpan.FromSeconds(request.Timeout);

        if (request.Timeout > _maxTimeout)
            httpClient.Timeout = TimeSpan.FromSeconds(_maxTimeout);

        if (request.Headers != null)
            foreach (var header in request.Headers)
                requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);

        requestMessage.Method = new HttpMethod(request.Method);

        var contentData = string.Empty;

        if (request.Body is JsonElement convertedContent)
            contentData = convertedContent.ValueKind != JsonValueKind.String
                ? JsonSerializer.Serialize(request.Body)
                : request.Body.ToString();
        else if (request.Body is Dictionary<string, object>)
            contentData = JsonSerializer.Serialize(request.Body);

        var content = new StringContent(contentData, Encoding.UTF8, "application/json");
        requestMessage.Content = content;

        var response = await httpClient.SendAsync(requestMessage);

        var isSuccess = response.StatusCode < HttpStatusCode.InternalServerError &&
                        response.StatusCode != HttpStatusCode.TooManyRequests &&
                        response.StatusCode != HttpStatusCode.RequestTimeout;

        return new HttpResponseDto
        {
            IsSuccess = isSuccess,
            StatusCode = (int)response.StatusCode,
            Response = await response.Content.ReadAsStringAsync()
        };
    }
}
