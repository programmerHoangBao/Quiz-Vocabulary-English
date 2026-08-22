using back_end.DTOs.Topic.Requests;
using back_end.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace back_end.Controllers
{
    [ApiController]
    [Route("api/topics")]
    [Authorize]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _topicService;
        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTopicById(Guid id)
        {
            var response = await _topicService.GetTopicById(id);
            return StatusCode(response.HttpStatusCode, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetTopicsByFolderId(
            [FromQuery] Guid folderId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20
        )
        {
            var response = await _topicService.GetTopicsByFolderId(folderId, pageNumber, pageSize);
            return StatusCode(response.HttpStatusCode, response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTopic(CreateTopicRequest req)
        {
            var response = await _topicService.CreateTopic(req);
            return StatusCode(response.HttpStatusCode, response);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateTopic(UpdateTopicRequest req)
        {
            var response = await _topicService.UpdateTopic(req);
            return StatusCode(response.HttpStatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteTopicById(Guid id)
        {
            var response = await _topicService.SoftDeleteById(id);
            return StatusCode(response.HttpStatusCode, response);
        }
    }
}
