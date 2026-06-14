using Conqueco.Models;
using Conqueco.Services.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Conqueco.DTOs.Auth;

namespace Conqueco.Controllers.Api
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtService _jwtService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            JwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                Pseudo = dto.Pseudo
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // 🖼️ AVATAR SAVE (si fourni)
            if (!string.IsNullOrEmpty(dto.AvatarBase64))
            {
                var fileName = $"{Guid.NewGuid()}.png";
                var path = Path.Combine("wwwroot/avatars", fileName);

                var bytes = Convert.FromBase64String(dto.AvatarBase64);

                System.IO.File.WriteAllBytes(path, bytes);

                user.AvatarUrl = "/avatars/" + fileName;
                await _userManager.UpdateAsync(user);
            }

            return Ok("User created");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized();

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

            if (!result.Succeeded)
                return Unauthorized();

            var token = _jwtService.GenerateToken(user);

            return Ok(new { token });
        }
    }
}