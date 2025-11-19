using Microsoft.AspNetCore.Mvc;
using OurPlan.Data;
using OurPlan.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using OurPlan.DTO;
using Microsoft.AspNetCore.Authorization;

namespace OurPlan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
        
    public class UserController : ControllerBase
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
            _context = context;
        }


        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = _currentUserService.UserId;

            if (userId == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }
            
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserModel
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Email = u.Email,
                        CreatedAt = u.CreatedAt
                    })
                .SingleOrDefaultAsync();


            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            return Ok(user);
        }
        
    }
    
    


}
