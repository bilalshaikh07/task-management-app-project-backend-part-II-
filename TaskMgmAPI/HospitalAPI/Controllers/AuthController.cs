using TaskMgmAPI.DTOfile;
using TaskMgmAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TaskMgmAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TaskContext _context;
        private readonly IConfiguration _config;

        public AuthController(TaskContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        //register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                return BadRequest("User already exists!");

            var user = new User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                Organization = registerDto.Organization,
                Designation = registerDto.Designation,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registration successful!" });
        }

        // login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");

            var token = GenerateJwtToken(user);

            //Return token + user info
            return Ok(new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                id = user.Id,
                FullName = user.FullName,
                Designation = user.Designation
            });
        }

        //  Private: Generate JWT Token
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim("Email", user.Email),
                new Claim("FullName", user.FullName),
                new Claim("Designation", user.Designation ?? "")
            };

            var secretKey = _config["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Length < 32)
                throw new Exception("JWT Key must be at least 32 characters long.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2), // Token valid for 2 hours
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Update user details by Email

    }
}
