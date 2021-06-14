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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request">Event information</param>
        /// <returns></returns>
        /// <response code="200">Event published to message broker.</response>
        /// <response code="400">Validation problems</response>
        [HttpPost]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        public async Task<IActionResult> PostAsync([FromBody] QueueRequest request)
        {
            var response = await _queueService.PublishAsync(new QueueRequestDto
            {
                EventName = request.EventName,
                Payload = request.Payload
            });
            return response ? Ok() : BadRequest();
        }
    }
}