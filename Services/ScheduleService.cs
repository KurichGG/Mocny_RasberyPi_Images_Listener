using Mocny_RasberyPi_Images_Listener.Data;
using Mocny_RasberyPi_Images_Listener.DTOs;
using Mocny_RasberyPi_Images_Listener.Models;
using Microsoft.EntityFrameworkCore;

namespace Mocny_RasberyPi_Images_Listener.Services
{
    public class ScheduleService
    {
        private readonly AppDbContext _context;

        private readonly MqttPublisherService _mqttService;

        public ScheduleService(AppDbContext context, MqttPublisherService mqttService)
        {
            _context = context;
            _mqttService = mqttService;
        }

      public async Task<List<ScheduleDto>> GetAllSchedules()
        {
            return await _context.Schedules
                .Include(s => s.Screen)
                .Select(s => new ScheduleDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    ImageId = s.ImageId,
                    CollectionId = s.CollectionId,
                    ScreenId = s.ScreenId,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    IsRecurring = s.IsRecurring,
                    RecurrencePattern = s.RecurrencePattern,
                    Priority = s.Priority
                })
                .ToListAsync();
        }

        public async Task<ScheduleDto?> GetScheduleById(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null) return null;

            return new ScheduleDto
            {
                Id = schedule.Id,
                Name = schedule.Name,
                ImageId = schedule.ImageId,
                CollectionId = schedule.CollectionId,
                ScreenId = schedule.ScreenId,
                StartDate = schedule.StartDate,
                EndDate = schedule.EndDate,
                IsRecurring = schedule.IsRecurring,
                RecurrencePattern = schedule.RecurrencePattern,
                Priority = schedule.Priority
            };
        }

        public async Task<ScheduleDto> CreateSchedule(CreateScheduleRequest request)
        {
            var schedule = new Schedule
            {
                Name = request.Name,
                ImageId = request.ImageId,
                CollectionId = request.CollectionId,
                ScreenId = request.ScreenId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsRecurring = request.IsRecurring,
                RecurrencePattern = request.RecurrencePattern,
                Priority = request.Priority,
                CreatedAt = DateTime.UtcNow
            };

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            return new ScheduleDto
            {
                Id = schedule.Id,
                Name = schedule.Name,
                ImageId = schedule.ImageId,
                CollectionId = schedule.CollectionId,
                ScreenId = schedule.ScreenId,
                StartDate = schedule.StartDate,
                EndDate = schedule.EndDate,
                IsRecurring = schedule.IsRecurring,
                RecurrencePattern = schedule.RecurrencePattern,
                Priority = schedule.Priority
            };
        }

        public async Task<bool> UpdateSchedule(int id, CreateScheduleRequest request)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null) return false;

            schedule.Name = request.Name;
            schedule.ImageId = request.ImageId;
            schedule.CollectionId = request.CollectionId;
            schedule.ScreenId = request.ScreenId;
            schedule.StartDate = request.StartDate;
            schedule.EndDate = request.EndDate;
            schedule.IsRecurring = request.IsRecurring;
            schedule.RecurrencePattern = request.RecurrencePattern;
            schedule.Priority = request.Priority;

            // Reset flag - edycja powinna umożliwić ponowne przetworzenie przez background service
            schedule.IsActivated = false;
            schedule.IsClosed = false;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSchedule(int id)
        {
            var schedule = await _context.Schedules
                .Include(s => s.Screen)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (schedule == null) return false;

            // Jeśli harmonogram był aktywny i jeszcze nie zamknięty - wyczyść ekran
            if (schedule.IsActivated && !schedule.IsClosed)
            {
                await _mqttService.PublishCommandAsync(schedule.Screen.UniqueIdentifier, new
                {
                    command = "clear"
                });
            }

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}