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

    [HttpGet("group/{groupId}")]
    public async Task<IActionResult> GetEventsForGroup(
        int groupId, 
        [FromQuery] string viewMode = "day", 
        [FromQuery] DateTime? date = null)
    {
        var result = await _eventService.GetEventsForGroup(groupId, viewMode, date);
        if(result.Result != null)
            return Ok(result.Result);
        return BadRequest(result.ValidationMessage);
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

    /// <summary>
    /// Exportă evenimentele din următoarele 30 de zile în format iCal (.ics)
    /// </summary>
    [HttpGet("export/ical")]
    public async Task<IActionResult> ExportToICal([FromQuery] int? groupId = null)
    {
        var result = await _eventService.ExportEventsToICal(groupId);
        if (result.Result != null)
        {
            var fileName = groupId.HasValue 
                ? $"ourplan-group-{groupId}-events.ics" 
                : "ourplan-events.ics";
            
            return File(
                System.Text.Encoding.UTF8.GetBytes(result.Result),
                "text/calendar",
                fileName);
        }
        return BadRequest(result.ValidationMessage);
    }

    /// <summary>
    /// Importă evenimente din format iCal (.ics) - doar evenimentele din următoarele 30 de zile
    /// </summary>
    [HttpPost("import/ical")]
    public async Task<IActionResult> ImportFromICal(
        [FromForm] IFormFile file,
        [FromQuery] int? groupId = null)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Fișierul nu a fost încărcat sau este gol.");
        }

        if (!file.FileName.EndsWith(".ics", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Fișierul trebuie să fie în format .ics");
        }

        try
        {
            using var reader = new StreamReader(file.OpenReadStream());
            var icalContent = await reader.ReadToEndAsync();

            var result = await _eventService.ImportEventsFromICal(icalContent, groupId);
            if (result.Result != null)
            {
                return Ok(new
                {
                    message = $"Au fost importate {result.Result.Count} evenimente.",
                    events = result.Result
                });
            }
            return BadRequest(result.ValidationMessage);
        }
        catch (Exception ex)
        {
            return BadRequest($"Eroare la citirea fișierului: {ex.Message}");
        }
    }

    /// <summary>
    /// Importă evenimente din format iCal (.ics) - versiune cu body direct
    /// </summary>
    [HttpPost("import/ical/raw")]
    public async Task<IActionResult> ImportFromICalRaw(
        [FromBody] string icalContent,
        [FromQuery] int? groupId = null)
    {
        if (string.IsNullOrWhiteSpace(icalContent))
        {
            return BadRequest("Conținutul iCal nu poate fi gol.");
        }

        var result = await _eventService.ImportEventsFromICal(icalContent, groupId);
        if (result.Result != null)
        {
            return Ok(new
            {
                message = $"Au fost importate {result.Result.Count} evenimente.",
                events = result.Result
            });
        }
        return BadRequest(result.ValidationMessage);
    }
}
