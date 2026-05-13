using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mocny_RasberyPi_Images_Listener.DTOs;
using Mocny_RasberyPi_Images_Listener.Services;

namespace Mocny_RasberyPi_Images_Listener.Controllers
{
    [ApiController]
    [Route("api/schedules")]
    [Authorize]
    public class SchedulesController : ControllerBase
    {
        private readonly ScheduleService _scheduleService;

        public SchedulesController(ScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSchedules()
        {
            var schedules = await _scheduleService.GetAllSchedules();
            return Ok(schedules);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetScheduleById(int id)
        {
            var schedule = await _scheduleService.GetScheduleById(id);
            if (schedule == null)
                return NotFound(new { message = "Schedule not found" });

            return Ok(schedule);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleRequest request)
        {
            if (string.IsNullOrEmpty(request.Name) || request.ScreenId == 0)
                return BadRequest(new { message = "Name and ScreenId required" });

            var schedule = await _scheduleService.CreateSchedule(request);
            return CreatedAtAction(nameof(GetScheduleById), new { id = schedule.Id }, schedule);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] CreateScheduleRequest request)
        {
            var result = await _scheduleService.UpdateSchedule(id, request);
            if (!result)
                return NotFound(new { message = "Schedule not found" });

            var updated = await _scheduleService.GetScheduleById(id);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var result = await _scheduleService.DeleteSchedule(id);
            if (!result)
                return NotFound(new { message = "Schedule not found" });

            return NoContent();
        }
    }
}