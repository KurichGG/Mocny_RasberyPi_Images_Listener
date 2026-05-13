namespace Mocny_RasberyPi_Images_Listener.Models
{
    public class CollectionItem
    {
        public int Id { get; set; }
        public int CollectionId { get; set; }
        public Collection Collection { get; set; } = null!;
        public int ImageId { get; set; }
        public Image Image { get; set; } = null!;
        public int Order { get; set; }
        public int DisplayDurationSeconds { get; set; } = 5;
    }
}