using back_end.DTOs.Folder.Requests;
using back_end.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace back_end.Controllers
{
    [ApiController]
    [Route("api/folders")]
    [Authorize]
    public class FolderController : ControllerBase
    {
        private readonly IFolderService _folderService;

        public FolderController(IFolderService folderService)
        {
            _folderService = folderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFoldersByUserId(
            [FromQuery] Guid userId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20
        )
        {
            var response = await _folderService.GetFoldersByUserId(userId, pageNumber, pageSize);
            return StatusCode(response.HttpStatusCode, response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFolder([FromBody] CreateFolderRequest req)
        {
            var response = await _folderService.CreateFolder(req);
            return StatusCode(response.HttpStatusCode, response);
        }
        [HttpPatch]
        public async Task<IActionResult> UpdateFolder([FromBody] UpdateFolderRequest req)
        {
            var response = await _folderService.UpdateFolder(req);
            return StatusCode(response.HttpStatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteFolder(Guid id)
        {
            var response = await _folderService.SoftDeleteFolder(id);
            return StatusCode(response.HttpStatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFolderById(Guid Id)
        {
            var response = await _folderService.GetFolderById(Id);
            return StatusCode(response.HttpStatusCode, response);
        }
    }
}
