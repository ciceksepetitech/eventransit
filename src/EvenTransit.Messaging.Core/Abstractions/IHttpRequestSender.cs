using EvenTransit.Messaging.Core.Dto;

namespace EvenTransit.Messaging.Core.Abstractions;

public interface IHttpRequestSender
{
    Task<HttpResponseDto> SendAsync(HttpRequestDto request);
}
