namespace Conqueco.DTOs.Announcement
{
    public class CreateAnnouncementDto
    {
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public bool IsPinned { get; set; }

        public bool IsPublished { get; set; }

        public string? ImageBase64 { get; set; }
    }
}