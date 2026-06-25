using Conqueco.DTOs.Announcement;
using Conqueco.Models;

namespace Conqueco.Services.Announcement
{
    public interface IAnnouncementService
    {
        Task<List<Conqueco.Models.Announcement>> GetAll();
        Task<Conqueco.Models.Announcement?> GetById(int id);
        Task<List<Conqueco.Models.Announcement>> GetPublished();
        Task Create(CreateAnnouncementDto dto, string userId);
        Task Update(UpdateAnnouncementDto dto);
        Task Delete(int id);
    }
}
