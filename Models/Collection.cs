namespace Mocny_RasberyPi_Images_Listener.Models
{
    public class Collection
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<CollectionItem> Items { get; set; } = new List<CollectionItem>();
    }
}