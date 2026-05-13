using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mocny_RasberyPi_Images_Listener.DTOs;
using Mocny_RasberyPi_Images_Listener.Services;

namespace Mocny_RasberyPi_Images_Listener.Controllers
{
    [ApiController]
    [Route("api/screens")]
    [Authorize]
    public class ScreensController : ControllerBase
    {
        private readonly ScreenService _screenService;

        public ScreensController(ScreenService screenService)
        {
            _screenService = screenService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllScreens()
        {
            var screens = await _screenService.GetAllScreens();
            return Ok(screens);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetScreenById(int id)
        {
            var screen = await _screenService.GetScreenById(id);
            if (screen == null)
                return NotFound(new { message = "Screen not found" });

            return Ok(screen);
        }

        [HttpPost]
        public async Task<IActionResult> CreateScreen([FromBody] CreateScreenRequest request)
        {
            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.UniqueIdentifier))
                return BadRequest(new { message = "Name and UniqueIdentifier required" });

            var screen = await _screenService.CreateScreen(request);
            return CreatedAtAction(nameof(GetScreenById), new { id = screen.Id }, screen);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateScreen(int id, [FromBody] CreateScreenRequest request)
        {
            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.UniqueIdentifier))
                return BadRequest(new { message = "Name and UniqueIdentifier required" });

            var result = await _screenService.UpdateScreen(id, request);
            if (!result)
                return NotFound(new { message = "Screen not found" });

            var updated = await _screenService.GetScreenById(id);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScreen(int id)
        {
            var result = await _screenService.DeleteScreen(id);
            if (!result)
                return NotFound(new { message = "Screen not found" });

            return NoContent();
        }

        [HttpPost("{id}/power")]
        public async Task<IActionResult> SetScreenPower(int id, [FromBody] SetPowerRequest request)
        {
            var result = await _screenService.SetScreenPower(id, request.IsOnline);
            if (!result)
                return NotFound(new { message = "Screen not found" });

            var screen = await _screenService.GetScreenById(id);
            return Ok(screen);
        }
    }

    public class SetPowerRequest
    {
        public bool IsOnline { get; set; }
    }
}