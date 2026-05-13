namespace Mocny_RasberyPi_Images_Listener.Models
{
    public class Screen
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UniqueIdentifier { get; set; } = string.Empty; // e.g., "screen-001"
        public int? GroupId { get; set; }
        public ScreenGroup? Group { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = "Offline"; // "Online" or "Offline"
        public DateTime? LastSeen { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}