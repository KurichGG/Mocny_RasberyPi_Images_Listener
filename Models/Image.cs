namespace Mocny_RasberyPi_Images_Listener.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string? ThumbnailPath { get; set; }
        public string Format { get; set; } = string.Empty; // e.g., "jpg", "png"
        public int Width { get; set; }
        public int Height { get; set; }
        public long FileSize { get; set; } // in bytes
        public int? UploadedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}