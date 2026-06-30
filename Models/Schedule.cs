namespace Mocny_RasberyPi_Images_Listener.Models
{
    public class Schedule
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ImageId { get; set; }
        public Image? Image { get; set; }
        public int? CollectionId { get; set; }
        public Collection? Collection { get; set; }
        public int ScreenId { get; set; }
        public Screen Screen { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsRecurring { get; set; }
        public string? RecurrencePattern { get; set; } // e.g., "daily", "weekly", "monthly"
        public int Priority { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActivated { get; set; } = false;
    }
}