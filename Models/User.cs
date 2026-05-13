namespace Mocny_RasberyPi_Images_Listener.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Operator"; // "Operator" or "Admin"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}