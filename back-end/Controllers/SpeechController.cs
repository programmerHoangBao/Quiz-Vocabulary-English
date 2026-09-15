using back_end.DTOs.Speech.Requests;
using back_end.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace back_end.Controllers
{
    [ApiController]
    [Route("api/speech")]
    [Authorize]
    public class SpeechController : ControllerBase
    {
        private readonly ISpeechService _speechService;
        public SpeechController(ISpeechService speechService)
        {
            _speechService = speechService;
        }
        [HttpPost("text-to-speech")]
        public async Task<IActionResult> ConvertTextToSpeed([FromBody] TextToSpeechRequest req)
        {
            var response = await _speechService.ConvertTextToSpeechAsync(req.Text);
            return StatusCode(response.HttpStatusCode, response);
        }
    }
}
