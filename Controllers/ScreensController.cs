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
        private readonly MqttPublisherService _mqttService;

        public ScreensController(ScreenService screenService, MqttPublisherService mqttService)
        {
            _screenService = screenService;
            _mqttService = mqttService;
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

        [HttpPost("{id}/display-image")]
        public async Task<IActionResult> DisplayImage(int id, [FromBody] DisplayImageRequest request)
        {
            var screen = await _screenService.GetScreenById(id);
            if (screen == null)
                return NotFound(new { message = "Screen not found" });

            await _mqttService.PublishCommandAsync(screen.UniqueIdentifier, new
            {
                command = "show_image",
                imageId = request.ImageId
            });

            return Ok(new { message = "Polecenie wysłane" });
        }

        [HttpPost("{id}/power")]
        public async Task<IActionResult> PowerControl(int id, [FromBody] PowerRequest request)
        {
            var screen = await _screenService.GetScreenById(id);
            if (screen == null)
                return NotFound(new { message = "Screen not found" });

            // Wyślij polecenie przez MQTT do Raspberry Pi
            await _mqttService.PublishCommandAsync(screen.UniqueIdentifier, new
            {
                command = request.PowerOn ? "power_on" : "power_off"
            });

            // Zaktualizuj status w bazie danych
            await _screenService.SetScreenPower(id, request.PowerOn);

            var updated = await _screenService.GetScreenById(id);
            return Ok(updated);
        }
    }

    public class DisplayImageRequest
    {
        public int ImageId { get; set; }
    }

    public class PowerRequest
    {
        public bool PowerOn { get; set; }
    }
}