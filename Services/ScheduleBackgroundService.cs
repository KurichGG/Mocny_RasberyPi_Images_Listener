using Microsoft.EntityFrameworkCore;
using Mocny_RasberyPi_Images_Listener.Data;

namespace Mocny_RasberyPi_Images_Listener.Services
{
    public class ScheduleBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ScheduleBackgroundService> _logger;
        private readonly IConfiguration _configuration;

        public ScheduleBackgroundService(IServiceProvider serviceProvider, ILogger<ScheduleBackgroundService> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
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
            var publicUrl = _configuration["Backend:PublicUrl"];

            // --- Harmonogramy do aktywacji (StartDate nadszedł) ---
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
                        imageId = schedule.ImageId.Value,
                        imageUrl = $"{publicUrl}/api/images/{schedule.ImageId.Value}/file"
                    });
                }
                else if (schedule.CollectionId.HasValue)
                {
                    var items = await context.CollectionItems
                        .Include(ci => ci.Image)
                        .Where(ci => ci.CollectionId == schedule.CollectionId.Value)
                        .OrderBy(ci => ci.Order)
                        .Select(ci => new
                        {
                            imageId = ci.ImageId,
                            imageUrl = $"{publicUrl}/api/images/{ci.ImageId}/file",
                            order = ci.Order,
                            displayDurationSeconds = ci.DisplayDurationSeconds
                        })
                        .ToListAsync();

                    await mqttService.PublishCommandAsync(schedule.Screen.UniqueIdentifier, new
                    {
                        command = "display_collection",
                        collectionId = schedule.CollectionId.Value,
                        items = items
                    });
                }

                schedule.IsActivated = true;
            }

            if (dueSchedules.Any())
            {
                await context.SaveChangesAsync();
            }

            // --- Harmonogramy do zamknięcia (EndDate minął) ---
            var schedulesToClose = await context.Schedules
                .Include(s => s.Screen)
                .Where(s => s.IsActivated
                         && !s.IsClosed
                         && s.EndDate < now)
                .ToListAsync();

            foreach (var schedule in schedulesToClose)
            {
                _logger.LogInformation($"Zamykam harmonogram: {schedule.Name} dla ekranu {schedule.Screen.UniqueIdentifier}");

                await mqttService.PublishCommandAsync(schedule.Screen.UniqueIdentifier, new
                {
                    command = "clear"
                });

                schedule.IsClosed = true;

                // Jeśli recurring – przygotuj do następnego cyklu
                if (schedule.IsRecurring)
                {
                    schedule.IsActivated = false;
                    schedule.IsClosed = false;
                    schedule.StartDate = ShiftDate(schedule.StartDate, schedule.RecurrencePattern);
                    schedule.EndDate = ShiftDate(schedule.EndDate, schedule.RecurrencePattern);
                }
            }

            if (schedulesToClose.Any())
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