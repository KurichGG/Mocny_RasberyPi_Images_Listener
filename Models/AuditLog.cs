namespace Mocny_RasberyPi_Images_Listener.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
        public string Action { get; set; } = string.Empty; // e.g., "CREATE", "UPDATE", "DELETE"
        public string EntityType { get; set; } = string.Empty; // e.g., "Screen", "Image"
        public int? EntityId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Description { get; set; } = string.Empty;
    }
}