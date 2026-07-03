using System.ComponentModel.DataAnnotations;

namespace WebPrincipal.Data.Entities
{
    public class Announcement
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public bool IsPinned { get; set; }

        public bool IsPublished { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public string CreatedById { get; set; } = string.Empty;

        public ApplicationUser? CreatedBy { get; set; }
    }
}