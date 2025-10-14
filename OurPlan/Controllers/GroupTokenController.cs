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

        [HttpPost("join")]
        public async Task<IActionResult> JoinGroupByToken([FromBody] string token)
        {
            var result = await _groupTokenService.JoinGroupByToken(token);

            if (result.Result != null)
                return Ok(new { message = "User successfully joined the group" });
            else
                return BadRequest(result.ValidationMessage);
        }


        [HttpGet("check/{groupId}")]
        public async Task<IActionResult> CheckUser(int groupId)
        {
            var result = await _groupTokenService.CheckUser(groupId);
            
            if (result.ValidationMessage.Any())
                return BadRequest(result.ValidationMessage);
            
            return Ok(new {isInGroup = result.Result});
            
        }
        


    }
}