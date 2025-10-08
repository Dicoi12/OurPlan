using Microsoft.AspNetCore.Mvc;
using OurPlan.Services.Interfaces;

namespace OurPlan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class GroupTokenController : ControllerBase
    {

        private readonly IGroupTokenService _groupTokenService;

        public GroupTokenController(IGroupTokenService groupTokenService)
        {
            _groupTokenService = groupTokenService;
        }


        [HttpGet("groupId")]

        public async Task<IActionResult> GenerateToken(int groupId)
        {
            var result = await _groupTokenService.GenerateToken(groupId);
            
            if(result.Result != null)
                return Ok(result.Result);
            return BadRequest();
        }
        
            


    }
}