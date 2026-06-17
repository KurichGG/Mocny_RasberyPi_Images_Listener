namespace Mocny_RasberyPi_Images_Listener.DTOs.Users
{
    public class CreateUserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Operator";
    }
}