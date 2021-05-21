using System.Threading.Tasks;
using EvenTransit.Api.Models;
using EvenTransit.Api.Validators;
using EvenTransit.Core.Abstractions.Service;
using EvenTransit.Core.Dto;
using Microsoft.AspNetCore.Mvc;

namespace EvenTransit.Api.Controllers
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

            var response = await _queueService.PublishAsync(new QueueRequestDto
                {
                    Name = name,
                    Payload = payload
                });
            return response ? Ok() : BadRequest(); 
        }
        
    }
}