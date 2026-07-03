using Microsoft.AspNetCore.Identity;

namespace WebPrincipal.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Pseudo { get; set; } = "Unknown user";

        public string? AvatarUrl { get; set; }

        public int Level { get; set; } = 1;
        public int XP { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}