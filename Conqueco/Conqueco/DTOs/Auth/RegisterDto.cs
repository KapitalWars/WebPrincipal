namespace Conqueco.DTOs.Auth
{
    public class RegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public string Pseudo { get; set; }

        public string? AvatarBase64 { get; set; } // image envoyée depuis frontend
    }
}