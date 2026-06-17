namespace Mocny_RasberyPi_Images_Listener.DTOs
{
    public class CollectionItemDto
    {
        public int Id { get; set; }
        public int ImageId { get; set; }
        public string ImageName { get; set; } = string.Empty;
        public string? ThumbnailPath { get; set; }
        public int Order { get; set; }
        public int DisplayDurationSeconds { get; set; }
    }
}