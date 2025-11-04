using OurPlan.Entity;
using OurPlan.Services.Interfaces;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using OurPlan.Data;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace OurPlan.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly ICurrentUserService _currentUserService;


        public UserService(ApplicationDbContext context, IConfiguration config, ICurrentUserService currentUserService)
        {
            _context = context;
            _config = config;
            _currentUserService = currentUserService;
        }

        public async Task<User> Register(string username, string email, string password)
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
            await _context.SaveChangesAsync();

            var group = new Group
            {
                Name = $"{username}'s Group",
                CreatedByUserId = user.Id
            };

            var groupUser = new UserGroup
            {
                UserId = user.Id,
                GroupId = group.Id,
                Role = "Owner"
            };

            _context.Groups.Add(group);
            _context.UserGroups.Add(groupUser);
            await _context.SaveChangesAsync();

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
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, user.Username)
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
