using OurPlan.Entity;
using OurPlan.Services.Interfaces;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using OurPlan.Data;

namespace OurPlan.Services
{
        public class UserService : IUserService
        {
            private readonly ApplicationDbContext _context;
            private readonly IConfiguration _config;

            public UserService(ApplicationDbContext context, IConfiguration config)
            {
                _context = context;
                _config = config;
            }

            public User Register(string username, string email, string password)
            {
                if (_context.Users.Any(u => u.Email == email))
                    throw new Exception("Email already exists");

                var user = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                return user;
            }

            public string Login(string email, string password)
            {
                var user = _context.Users.SingleOrDefault(u => u.Email == email);
                if (user == null)
                    throw new Exception("Invalid credentials");

                if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                    throw new Exception("Invalid credentials");

                return GenerateJwtToken(user);
            }

            private string GenerateJwtToken(User user)
            {
                var jwtSettings = _config.GetSection("Jwt");
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username)
            };

                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"])),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
        
    }
}
