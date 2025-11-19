using Microsoft.AspNetCore.Mvc;
using OurPlan.Services.Interfaces;

namespace OurPlan.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventController : ControllerBase
{
    private readonly IEventService _eventService;
    public EventController(IEventService eventService)
    {
        this._eventService = eventService;
    }

    [HttpGet]
    
    public async Task<IActionResult> GetAll ()
    {
        var events = await _eventService.GetEventsForCurrentUser();
        return Ok(events);
    }

    [HttpGet("group/{groupId}/today")]

    public async Task<IActionResult> GetEventsForGroup(int groupId)
    {
        var result = await _eventService.GetEventsForGroup(groupId);
        if(result.Result != null)
            return Ok(result.Result);
        return BadRequest();
    }
    
    

    [HttpPost]
    public IActionResult CreateEvent([FromBody] DTO.EventModel model)
    {
        var result = _eventService.CreateEvent(model);
        if (result.Result != null)
            return Ok(result.Result);
        return BadRequest();
    }

    [HttpPut]
    public IActionResult UpdateEvent([FromBody] DTO.EventModel model)
    {
        var result = _eventService.UpdateEvent(model);
        if (result.Result != null)
            return Ok(result.Result);
        return BadRequest();
    }
    
    [HttpDelete("{eventId}")] 
    public IActionResult DeleteEvent(int eventId)
    {
        var result = _eventService.DeleteEvent(eventId);
        if (result.Result != null)
            return Ok(result.Result);
        return BadRequest();
    }
}
