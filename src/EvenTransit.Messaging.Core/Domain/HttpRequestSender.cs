using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EvenTransit.Messaging.Core.Abstractions;
using EvenTransit.Messaging.Core.Dto;

namespace EvenTransit.Messaging.Core.Domain
{
    public class HttpRequestSender : IHttpRequestSender
    {
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

            if (request.Timeout > 0) httpClient.Timeout = TimeSpan.FromSeconds(request.Timeout);

            if (request.Headers != null)
            {
                foreach (var header in request.Headers)
                {
                    requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            requestMessage.Method = HttpMethod.Post;

            var content = new StringContent(request.Body, Encoding.UTF8, "application/json");
            requestMessage.Content = content;

            var response = await httpClient.SendAsync(requestMessage);

            return new HttpResponseDto
            {
                IsSuccess = response.IsSuccessStatusCode,
                StatusCode = (int) response.StatusCode,
                Response = await response.Content.ReadAsStringAsync()
            };
        }
    }
}