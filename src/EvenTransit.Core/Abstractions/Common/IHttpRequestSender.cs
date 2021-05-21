using System.Threading.Tasks;
using EvenTransit.Core.Dto;

namespace EvenTransit.Core.Abstractions.Common
{
    public interface IHttpRequestSender
    {
        Task<HttpResponseDto> SendAsync(HttpRequestDto request);
    }
}