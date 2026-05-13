namespace Mocny_RasberyPi_Images_Listener.DTOs
{
    public class ImageDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
        public long FileSize { get; set; }
        public string? ThumbnailPath { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}