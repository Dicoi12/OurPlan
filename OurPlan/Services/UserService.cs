using OurPlan.Entity;
using OurPlan.Services.Interfaces;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using OurPlan.Data;
using Microsoft.AspNetCore.Http;

namespace OurPlan.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public UserService(ApplicationDbContext context, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
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

        public User GetCurrentUser()
        {
            var bearerToken = GetBearerTokenFromHeader();
            if (string.IsNullOrWhiteSpace(bearerToken))
                throw new ArgumentException("Bearer token is required.");

            // Remove "Bearer " prefix if present
            var token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;

            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = key,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub);
                if (userIdClaim == null)
                    throw new Exception("User ID claim not found in token.");

                int userId = int.Parse(userIdClaim.Value);
                var user = _context.Users.SingleOrDefault(u => u.Id == userId);
                if (user == null)
                    throw new Exception("User not found.");

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid token.", ex);
            }
        }
        private string? GetBearerTokenFromHeader()
        {
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader != null && authHeader.StartsWith("Bearer "))
                return authHeader.Substring("Bearer ".Length);
            return null;
        }
    }
}
