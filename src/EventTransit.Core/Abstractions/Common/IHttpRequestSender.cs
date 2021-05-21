using System.Threading.Tasks;
using EventTransit.Core.Dto;

namespace EventTransit.Core.Abstractions.Common
{
    public interface IHttpRequestSender
    {
        Task<HttpResponseDto> SendAsync(HttpRequestDto request);
    }
}