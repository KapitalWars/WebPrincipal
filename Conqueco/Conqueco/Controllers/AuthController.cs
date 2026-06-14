using Conqueco.DTOs.Auth;
using Conqueco.Models;
using Conqueco.Services.Auth;
using Conqueco.Services.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Conqueco.Controllers.Api
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtService _jwtService;
        private readonly EmailService _emailService;
        private readonly IConfiguration _config;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            JwtService jwtService,
            EmailService emailService,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _emailService = emailService;
            _config = config;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
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

        [HttpGet("/debug-user")]
        public IActionResult DebugUser()
        {
            return Ok(new
            {
                isAuth = User.Identity?.IsAuthenticated,
                name = User.Identity?.Name
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized();

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName,
                dto.Password,
                isPersistent: true,
                lockoutOnFailure: false
            );

            if (!result.Succeeded)
                return Unauthorized();

            await _signInManager.SignInAsync(user, isPersistent: true);

            return Ok(new { message = "Logged in" });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Ok(); // sécurité: ne pas leak user existence

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = $"http://localhost:8080/AuthPage/ResetPassword?email={email}&token={Uri.EscapeDataString(token)}";

            await _emailService.SendAsync(
                email,
                "Reset your password",
                $"<p>Click here to reset your password:</p><a href='{resetLink}'>Reset Password</a>"
            );

            return Ok();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return BadRequest();

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok();
        }
    }
}