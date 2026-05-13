using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mocny_RasberyPi_Images_Listener.Services;

namespace Mocny_RasberyPi_Images_Listener.Controllers
{
    [ApiController]
    [Route("api/logs")]
    [Authorize]
    public class LogsController : ControllerBase
    {
        private readonly LogService _logService;

        public LogsController(LogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLogs([FromQuery] int? limit)
        {
            var logs = await _logService.GetAllLogs(limit);
            return Ok(logs);
        }

        [HttpGet("screen/{screenId}")]
        public async Task<IActionResult> GetLogsByScreen(int screenId, [FromQuery] int? limit)
        {
            var logs = await _logService.GetLogsByScreen(screenId, limit);
            return Ok(logs);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetLogsByUser(int userId, [FromQuery] int? limit)
        {
            var logs = await _logService.GetLogsByUser(userId, limit);
            return Ok(logs);
        }

        [HttpGet("action/{action}")]
        public async Task<IActionResult> GetLogsByAction(string action, [FromQuery] int? limit)
        {
            var logs = await _logService.GetLogsByAction(action, limit);
            return Ok(logs);
        }
    }
}