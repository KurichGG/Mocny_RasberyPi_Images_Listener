using Microsoft.EntityFrameworkCore;
using Mocny_RasberyPi_Images_Listener.Data;

namespace Mocny_RasberyPi_Images_Listener.Services
{
    public class ScheduleBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ScheduleBackgroundService> _logger;

        public ScheduleBackgroundService(IServiceProvider serviceProvider, ILogger<ScheduleBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ScheduleBackgroundService uruchomiony");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckSchedulesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Błąd w ScheduleBackgroundService");
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        private async Task CheckSchedulesAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var mqttService = scope.ServiceProvider.GetRequiredService<MqttPublisherService>();

            var now = DateTime.UtcNow;

            // Znajdź harmonogramy, których czas właśnie nadszedł i nie zostały jeszcze aktywowane
            var dueSchedules = await context.Schedules
                .Include(s => s.Screen)
                .Where(s => !s.IsActivated
                         && s.StartDate <= now
                         && s.EndDate >= now)
                .ToListAsync();

            foreach (var schedule in dueSchedules)
            {
                _logger.LogInformation($"Aktywuję harmonogram: {schedule.Name} dla ekranu {schedule.Screen.UniqueIdentifier}");

                if (schedule.ImageId.HasValue)
                {
                    await mqttService.PublishCommandAsync(schedule.Screen.UniqueIdentifier, new
                    {
                        command = "show_image",
                        imageId = schedule.ImageId.Value
                    });
                }
                else if (schedule.CollectionId.HasValue)
                {
                    await mqttService.PublishCommandAsync(schedule.Screen.UniqueIdentifier, new
                    {
                        command = "show_collection",
                        collectionId = schedule.CollectionId.Value
                    });
                }

                schedule.IsActivated = true;
            }

            if (dueSchedules.Any())
            {
                await context.SaveChangesAsync();
            }

            // Resetuj harmonogramy cykliczne, których czas już minął (EndDate < now) i są recurring
            var expiredRecurring = await context.Schedules
                .Where(s => s.IsActivated && s.IsRecurring && s.EndDate < now)
                .ToListAsync();

            foreach (var schedule in expiredRecurring)
            {
                // Przesuń datę o jeden dzień/tydzień/miesiąc w zależności od wzorca
                schedule.IsActivated = false;
                schedule.StartDate = ShiftDate(schedule.StartDate, schedule.RecurrencePattern);
                schedule.EndDate = ShiftDate(schedule.EndDate, schedule.RecurrencePattern);
            }

            if (expiredRecurring.Any())
            {
                await context.SaveChangesAsync();
            }
        }

        private DateTime ShiftDate(DateTime date, string? pattern)
        {
            return pattern switch
            {
                "daily" => date.AddDays(1),
                "weekly" => date.AddDays(7),
                "monthly" => date.AddMonths(1),
                _ => date
            };
        }
    }
}