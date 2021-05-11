using System.Threading.Tasks;
using EventTransit.Api.Models;
using EventTransit.Api.Validators;
using EventTransit.Core.Abstractions.Service;
using EventTransit.Core.Dto;
using Microsoft.AspNetCore.Mvc;

namespace EventTransit.Api.Controllers
{
    [ApiController]
    [Route("api/v1/queue")]
    public class QueueController : ControllerBase
    {
        private readonly IQueueService _queueService;

        public QueueController(IQueueService queueService)
        {
            _queueService = queueService;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromQuery] string name, [FromBody] dynamic payload)
        {
            var request = new QueueRequest {Name = name, Payload = payload};
            var validator = new QueueRequestValidator();

            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var response = _queueService.Publish(new QueueRequestDto
                {
                    Name = name,
                    Payload = payload
                });
            return response ? Ok() : BadRequest(); 
        }
        
    }
}