using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mocny_RasberyPi_Images_Listener.Services;
using System.Security.Claims;

namespace Mocny_RasberyPi_Images_Listener.Controllers
{
    [ApiController]
    [Route("api/images")]
    [Authorize]
    public class ImagesController : ControllerBase
    {
        private readonly ImageService _imageService;

        public ImagesController(ImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllImages()
        {
            var images = await _imageService.GetAllImages();
            return Ok(images);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                var userId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : (int?)null;
                var image = await _imageService.UploadImage(file, userId);
                return Ok(image);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Upload failed: " + ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var userId = int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid) ? uid : (int?)null;
            var result = await _imageService.DeleteImage(id, userId);
            if (!result)
                return NotFound(new { message = "Image not found" });

            return NoContent();
        }

        [HttpGet("{id}/file")]
        [AllowAnonymous]
        public async Task<IActionResult> GetImageFile(int id)
        {
            var fileStream = await _imageService.GetImageFile(id);
            if (fileStream == null)
                return NotFound();

            return File(fileStream, "image/jpeg");
        }
    }
}