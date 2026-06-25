using Conqueco.DTOs.Announcement;
using Conqueco.Services.Announcement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Conqueco.Controllers.Api
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/announcement")]
    public class AnnouncementController : ControllerBase
    {
        private readonly IAnnouncementService _service;

        public AnnouncementController(IAnnouncementService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _service.GetById(id));
        }

        [AllowAnonymous]
        [HttpGet("published")]
        public async Task<IActionResult> GetPublished()
        {
            var announcements = await _service.GetPublished();

            return Ok(announcements);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAnnouncementDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            await _service.Create(dto, userId);

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateAnnouncementDto dto)
        {
            await _service.Update(dto);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.Delete(id);

            return Ok();
        }
    }
}