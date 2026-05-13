namespace Mocny_RasberyPi_Images_Listener.DTOs
{
    public class CreateScreenRequest
    {
        public string Name { get; set; } = string.Empty;
        public string UniqueIdentifier { get; set; } = string.Empty;
        public int? GroupId { get; set; }
        public string Location { get; set; } = string.Empty;
    }
}