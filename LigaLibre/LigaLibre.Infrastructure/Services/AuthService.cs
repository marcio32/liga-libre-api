using LigaLibre.Application.DTOs;
using LigaLibre.Application.Interfaces;
using LigaLibre.EmailService.Interfaces;
using LigaLibre.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LigaLibre.EmailService.Models;

namespace LigaLibre.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;


        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration, IEmailService emailService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
        }


        public async Task<AuthResponseDto> LoginAsync(LoginDto login)
        {
            var user = await _userManager.FindByEmailAsync(login.Email);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password.");

            var isValidPassword = await _userManager.CheckPasswordAsync(user, login.Password);
            if (!isValidPassword)
                throw new UnauthorizedAccessException("Invalid email or password.");

            var token = await GenerateJwtTokenAsync(user.Email!);
            var roles = await _userManager.GetRolesAsync(user);
            try
            {
                await _emailService.SendEmailAsync(new EmailMessage
                {
                    To = user.Email!,
                    Subject = "Nuevo Inicio de Sesión Detectado",
                    Body = $"Hola {user.FirstName},\n\nHemos detectado un nuevo inicio de sesión en tu cuenta. Si no fuiste tú, por favor, inicia sesión de manera segura y cambia tu contraseña.\n\nGracias,\nEl Equipo de LigaLibre",
                    IsHtml = false
                });
            }
            catch  (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto register)
        {


            var existingUser = await _userManager.FindByEmailAsync(register.Email);
            if (existingUser != null)
                throw new UnauthorizedAccessException("User already exists.");

            var user = new ApplicationUser
            {
                UserName = register.Email,
                Email = register.Email,
                FirstName = register.FirstName,
                LastName = register.LastName,
            };

            var result = await _userManager.CreateAsync(user, register.Password);

            if(!result.Succeeded)
                throw new ArgumentException(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "User");

            var token = await GenerateJwtTokenAsync(user.Email!);
            var roles = await _userManager.GetRolesAsync(user);

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles
            };
        }

        public async Task<string> GenerateJwtTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if(user == null)
                throw new ArgumentException("User not found.");

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, user.Id),
                new (ClaimTypes.Email, user.Email ?? ""),
                new (ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new (ClaimTypes.Role, roles.FirstOrDefault() ?? "")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddYears(1),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
