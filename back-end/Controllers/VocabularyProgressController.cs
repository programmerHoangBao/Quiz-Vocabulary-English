using back_end.DTOs.VocabularyProgress.Requests;
using back_end.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace back_end.Controllers
{
    [ApiController]
    [Route("api/vocabulary-progress")]
    [Authorize]
    public class VocabularyProgressController : ControllerBase
    {
        private readonly IVocabularyProgressService _vocabularyProgressService;
        public VocabularyProgressController(IVocabularyProgressService vocabularyProgressService)
        {
            _vocabularyProgressService = vocabularyProgressService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateVocabularyProgress([FromBody] CreateVocabularyProgressRequest req)
        {
            var result = await _vocabularyProgressService.CreateVocabularyProgressAsync(req);
            return StatusCode(result.HttpStatusCode, result);
        }
        [HttpGet("due/{userId}")]
        public async Task<IActionResult> GetDueVocabularies(Guid userId)
        {
            var result = await _vocabularyProgressService.GetDueVocabulariesAsync(userId);
            return StatusCode(result.HttpStatusCode, result);
        }

        [HttpPost("submit-review")]
        public async Task<IActionResult> SubmitReview([FromBody] SubmitReviewRequest req)
        {
            var result = await _vocabularyProgressService.SubmitReviewAsync(req);
            return StatusCode(result.HttpStatusCode, result);
        }
    }
}
