using Microsoft.AspNetCore.Identity;

namespace Conqueco.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Champs personnalisés futurs
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
