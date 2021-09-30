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
        private const string FieldNameRegex = "{{([a-zA-Z0-9]+)}}";
        private readonly IHttpClientFactory _clientFactory;

        public HttpRequestSender(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<HttpResponseDto> SendAsync(HttpRequestDto request)
        {
            var requestMessage = new HttpRequestMessage();
            var httpClient = _clientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(ReplaceFieldsValue(request.Fields, request.Url));

            if (request.Timeout > 0) httpClient.Timeout = TimeSpan.FromSeconds(request.Timeout);

            if (request.Headers != null)
            {
                foreach (var header in request.Headers)
                {
                    requestMessage.Headers.TryAddWithoutValidation(header.Key, ReplaceFieldsValue(request.Fields, header.Value));
                }
            }

            requestMessage.Method = new HttpMethod(request.Method);
            
            var contentData = JsonSerializer.Serialize(request.Body);
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

        private string ReplaceFieldsValue(Dictionary<string, string> fields, string value)
        {
            var fieldRegex = new Regex(FieldNameRegex);
            var valueFields = fieldRegex.Matches(value);
            var newValue = value;

            foreach (Match field in valueFields)
            {
                var pattern = field.Groups[0].Value;
                var fieldName = field.Groups[1].Value;
                var newFieldValue = string.Empty;

                if (fields.ContainsKey(fieldName))
                    newFieldValue = fields[fieldName];

                newValue = newValue.Replace(pattern, newFieldValue);
            }

            return newValue;
        }
    }
}