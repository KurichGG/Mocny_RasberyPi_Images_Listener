namespace Mocny_RasberyPi_Images_Listener.DTOs
{
    public class CreateScheduleRequest
    {
        public string Name { get; set; } = string.Empty;
        public int? ImageId { get; set; }
        public int? CollectionId { get; set; }
        public int ScreenId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsRecurring { get; set; } = false;
        public string? RecurrencePattern { get; set; }
        public int Priority { get; set; } = 0;
    }
}