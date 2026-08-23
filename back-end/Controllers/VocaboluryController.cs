using back_end.DTOs.Vocabolury.Requests;
using back_end.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace back_end.Controllers
{
    [ApiController]
    [Route("api/vocaboluries")]
    [Authorize]
    public class VocaboluryController : ControllerBase
    {
        private readonly IVocaboluryService _vocaboluryService;

        public VocaboluryController(IVocaboluryService vocaboluryService)
        {
            _vocaboluryService = vocaboluryService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateVocabolury([FromBody] CreateVocaboluryRequest req)
        {
            var response = await _vocaboluryService.CreateVocabolury(req);
            return StatusCode(response.HttpStatusCode, response);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateVocabolury([FromBody,] UpdateVocaboluryRequest req)
        {
            var response = await _vocaboluryService.UpdateVocabolury(req);
            return StatusCode(response.HttpStatusCode, response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteVocabolury(Guid id)
        {
            var response = await _vocaboluryService.SoftDeleteById(id);
            return StatusCode(response.HttpStatusCode, response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVocaboluryById(Guid id)
        {
            var response = await _vocaboluryService.GetVocaboluryById(id);
            return StatusCode(response.HttpStatusCode, response);
        }
        [HttpGet]
        public async Task<IActionResult> GetVocaboluriesByTopicId(
            [FromQuery] Guid topicId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20
        )
        {
            var response = await _vocaboluryService.GetVocaboluriesByTopicId(topicId, pageNumber, pageSize);
            return StatusCode(response.HttpStatusCode, response);
        }
    }
}
