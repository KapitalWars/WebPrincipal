using Conqueco.Data;
using Conqueco.DTOs.Announcement;
using Conqueco.Models;
using Microsoft.EntityFrameworkCore;

namespace Conqueco.Services.Announcement
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly ApplicationDbContext _context;

        public AnnouncementService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Conqueco.Models.Announcement>> GetAll()
        {
            return await _context.Announcements
                .OrderByDescending(x => x.IsPinned)
                .ThenByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<Conqueco.Models.Announcement?> GetById(int id)
        {
            return await _context.Announcements.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Create(CreateAnnouncementDto dto, string userId)
        {
            string? imageUrl = null;

            if (!string.IsNullOrEmpty(dto.ImageBase64))
            {
                var fileName = $"{Guid.NewGuid()}.png";

                var path = Path.Combine("wwwroot", "announcements", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                await File.WriteAllBytesAsync(path, Convert.FromBase64String(dto.ImageBase64));

                imageUrl = "/announcements/" + fileName;
            }

            var entity = new Models.Announcement
            {
                Title = dto.Title,
                Content = dto.Content,
                IsPinned = dto.IsPinned,
                IsPublished = dto.IsPublished,
                ImageUrl = imageUrl,
                CreatedById = userId
            };

            _context.Announcements.Add(entity);

            await _context.SaveChangesAsync();
        }

        public async Task Update( UpdateAnnouncementDto dto)
        {
            var entity = await _context.Announcements.FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (entity == null)
                throw new Exception("Announcement not found");

            entity.Title = dto.Title;
            entity.Content = dto.Content;
            entity.IsPinned = dto.IsPinned;
            entity.IsPublished = dto.IsPublished;
            entity.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(dto.ImageBase64))
            {
                var fileName = $"{Guid.NewGuid()}.png";

                var path =Path.Combine("wwwroot", "announcements", fileName);

                await File.WriteAllBytesAsync(path, Convert.FromBase64String(dto.ImageBase64));

                entity.ImageUrl = "/announcements/" + fileName;
            }

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await _context.Announcements.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return;

            _context.Announcements.Remove(entity);

            await _context.SaveChangesAsync();
        }

        public async Task<List<Models.Announcement>> GetPublished()
        {
            return await _context.Announcements
                .Where(a => a.IsPublished)
                .OrderByDescending(a => a.IsPinned)
                .ThenByDescending(a => a.CreatedAt)
                .Select(a => new Models.Announcement
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = a.Content,
                    ImageUrl = a.ImageUrl,
                    IsPinned = a.IsPinned,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();
        }
    }
}