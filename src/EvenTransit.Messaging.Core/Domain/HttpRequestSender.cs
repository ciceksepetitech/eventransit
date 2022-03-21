using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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

            requestMessage.Method = new HttpMethod(request.Method);

            
            var contentData = string.Empty;

            if (request.Body is JsonElement convertedContent)
            {
                contentData = convertedContent.ValueKind != JsonValueKind.String 
                    ? JsonSerializer.Serialize(request.Body) 
                    : request.Body.ToString();
            }

            var content = new StringContent(contentData, Encoding.UTF8, "application/json");
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