using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace EvenTransit.IntegrationTests
{
    public class UnitTest1 : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public UnitTest1(TestWebApplicationFactory webApplicationFactory)
        {
            _client = webApplicationFactory.CreateClient();
        }
        
        [Fact]
        public async Task Test1()
        {
            var httpResponse = await _client.GetAsync("/");
            
            httpResponse.EnsureSuccessStatusCode();
            
            Assert.Equal(1, 1);
        }
    }
}