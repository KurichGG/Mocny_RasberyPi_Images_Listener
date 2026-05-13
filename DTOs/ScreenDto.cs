namespace Mocny_RasberyPi_Images_Listener.DTOs
{
    public class ScreenDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UniqueIdentifier { get; set; } = string.Empty;
        public int? GroupId { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? LastSeen { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}