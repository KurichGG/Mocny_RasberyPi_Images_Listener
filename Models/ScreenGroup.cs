namespace Mocny_RasberyPi_Images_Listener.Models
{
    public class ScreenGroup
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Screen> Screens { get; set; } = new List<Screen>();
    }
}