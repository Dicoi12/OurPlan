using Microsoft.AspNetCore.Mvc;
using OurPlan.DTO;
using OurPlan.Services.Interfaces;

namespace OurPlan.Controllers;

[ApiController]
[Route("api/[controller]")]

public class GroupController : ControllerBase
{
    private readonly IGroupService _groupService;

    public GroupController(IGroupService groupService)
    {
        this._groupService = groupService;
    }

    [HttpGet]
    public async Task<IActionResult> GetGroups()
    {
        var groups = await _groupService.GetGroupsForCurrentUser();
        return Ok(groups);
    }


    [HttpGet]
    [Route("GetAllGroups")]
    public async Task<ActionResult> GetAllGroups()
    {
        var result = await _groupService.GetAllGroups();
        if (result.Result != null)
            return Ok(result.Result);
        return BadRequest();
    }


    [HttpPost]
    public IActionResult CreateGroup([FromBody] DTO.GroupModel model)
    {
        var group = _groupService.CreateGroup(model);
        if (group.Result != null)
            return Ok(group.Result);
        return BadRequest();
    }

    [HttpPut]
    public IActionResult UpdateGroup([FromBody] GroupModel model)
    {
        var group = _groupService.UpdateGroup(model);
        if (group.Result != null)
            return Ok(group.Result);
        return BadRequest();
    }

    [HttpDelete]
    public IActionResult DeleteGroup(int groupId)
    {
        var group = _groupService.DeleteGroup(groupId);
        if (group.Result != null)
            return Ok(group.Result);
        else
            return BadRequest();
    }


}