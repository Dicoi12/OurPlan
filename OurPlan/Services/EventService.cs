using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OurPlan.Data;
using OurPlan.DTO;
using OurPlan.Entity;
using OurPlan.Helpers;
using OurPlan.Services.Interfaces;

namespace OurPlan.Services
{
    public class EventService : IEventService
    {
        public readonly ApplicationDbContext _context;
        public readonly ICurrentUserService _currentUserService;
        public readonly IMapper _mapper;
        public EventService(ApplicationDbContext context, ICurrentUserService userService, IMapper mapper)
        {
            _context = context;
            _currentUserService = userService;
            _mapper = mapper;
        }

        public async Task<ServiceResult<List<EventModel>>> GetEventsForGroup(int groupId, string viewMode, DateTime? date)
        {
            var result = new ServiceResult<List<EventModel>>();

            try
            {
                var currentUser = _currentUserService.UserId;
                if (currentUser == null)
                {
                    result.ValidationMessage.Add("User not authenticated");
                    return result;
                }

                var memberGroup = await _context.UserGroups
                    .AnyAsync(g => g.UserId == currentUser && g.GroupId == groupId);
                if (!memberGroup)
                {
                    result.ValidationMessage.Add("Nu ai permisiunea sa vezi evenimentele acestui grup");
                    return result;
                }

                var groupUserId = await _context.UserGroups
                    .Where(g => g.GroupId == groupId)
                    .Select(g => g.UserId)
                    .ToListAsync();

                var targetDate = date?.Date ?? DateTime.UtcNow.Date;
                //if (date.HasValue && date.Value.Kind != DateTimeKind.Utc)
                //{
                //    targetDate = date.Value.ToUniversalTime().Date;
                //}

                DateTime startDate;
                DateTime endDate;

                switch (viewMode?.ToLower())
                {
                    case "day":
                        startDate = targetDate;
                        endDate = targetDate.AddDays(1);
                        break;
                    case "week":
                        var dayOfWeek = (int)targetDate.DayOfWeek;
                        startDate = targetDate.AddDays(-dayOfWeek);
                        endDate = startDate.AddDays(7);
                        break;
                    case "month":
                        startDate = new DateTime(targetDate.Year, targetDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                        endDate = startDate.AddMonths(1);
                        break;
                    default:
                        startDate = targetDate;
                        endDate = targetDate.AddDays(1);
                        break;
                }

                if (startDate.Kind != DateTimeKind.Utc)
                {
                    startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
                }
                if (endDate.Kind != DateTimeKind.Utc)
                {
                    endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
                }

                var startDateUtc = startDate;
                var endDateUtc = endDate;

                var events = await _context.Events.Include(x => x.User)
                    .Where(e =>
                        groupUserId.Contains(e.CreatedByUserId) &&
                        (
                            // Evenimentul începe în interval
                            (e.StartDate >= startDateUtc && e.StartDate < endDateUtc) ||
                            // Evenimentul se termină în interval
                            (e.EndDate >= startDateUtc && e.EndDate < endDateUtc) ||
                            // Evenimentul cuprinde întreg intervalul
                            (e.StartDate <= startDateUtc && e.EndDate >= endDateUtc)
                        )
                    )
                    .ToListAsync();

                result.Result = _mapper.Map<List<EventModel>>(events);
            }
            catch (Exception ex)
            {
                result.ValidationMessage.Add($"An error occurred while fetching group events: {ex.Message}");
            }

            return result;
        }

        public async Task<ServiceResult<EventModel>> CreateEvent(EventModel model)
        {
            var result = new ServiceResult<EventModel>();
            try
            {
                var currentUser = _currentUserService.UserId;
                if (currentUser == null)
                {
                    result.ValidationMessage.Add("User not authenticated");
                    return result;
                }

                model.CreatedByUserId = (int)currentUser;
                _context.Events.Add(_mapper.Map<Event>(model));

                await _context.SaveChangesAsync();

                result.Result = model;
            }
            catch (Exception ex)
            {
                result.ValidationMessage.Add($"An error occurred while creating the event: {ex.Message}");
            }
            return result;
        }

        public async Task<ServiceResult<EventModel>> DeleteEvent(int eventId)
        {
            ServiceResult<EventModel> result = new();
            var currentUserId = _currentUserService.UserId;
            var entity = _context.Events.FirstOrDefault(e => e.Id == eventId && e.CreatedByUserId == currentUserId);
            if (entity == null)
            {
                result.ValidationMessage.Add("Event not found");
                return result;
            }

            _context.Remove(entity);
            await _context.SaveChangesAsync();

            return result;
        }

        public async Task<ServiceResult<List<EventModel>>> GetEventsForCurrentUser()
        {
            ServiceResult<List<EventModel>> result = new();
            try
            {
                var currentUser = _currentUserService.UserId;
                if (currentUser == null)
                {
                    result.ValidationMessage.Add("User not authenticated");
                    return result;
                }
                var events = await _context.Events.Include(x => x.User)
                    .Where(e => e.CreatedByUserId == currentUser)
                    .Select(e => new EventModel
                    {
                        Id = e.Id,
                        Title = e.Title,
                        Description = e.Description,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate,
                        IsShared = e.IsShared,
                        Location = e.Location
                    })
                    .ToListAsync();

                result.Result = events;
            }
            catch (Exception ex)
            {
                result.ValidationMessage.Add($"An error occurred while fetching events: {ex.Message}");
            }

            return result;

        }

        public async Task<ServiceResult<EventModel>> UpdateEvent(EventModel model)
        {
            var result = new ServiceResult<EventModel>();
            var exist = _context.Events.FirstOrDefault(e => e.Id == model.Id);

            if (exist == null)
            {
                result.ValidationMessage.Add("Event not found");
                return result;
            }

            try
            {
                exist.Title = model.Title;
                exist.Description = model.Description;
                exist.StartDate = model.StartDate;
                exist.EndDate = model.EndDate;
                exist.Location = model.Location;
                exist.IsShared = model.IsShared;
                exist.ReminderMinutesBefore = model.ReminderMinutesBefore;
                _context.Events.Update(exist);
                _context.SaveChanges();
                result.Result = model;
            }
            catch (Exception ex)
            {
                result.ValidationMessage.Add($"An error occurred while updating the event: {ex.Message}");
            }
            return result;
        }

        public async Task<ServiceResult<string>> ExportEventsToICal(int? groupId = null)
        {
            var result = new ServiceResult<string>();

            try
            {
                var currentUser = _currentUserService.UserId;
                if (currentUser == null)
                {
                    result.ValidationMessage.Add("User not authenticated");
                    return result;
                }

                // Calculăm intervalul: de la ziua curentă până la 30 de zile în viitor
                var startDate = DateTime.UtcNow.Date;
                var endDate = startDate.AddDays(30);

                List<EventModel> events;

                if (groupId.HasValue)
                {
                    // Export evenimente din grup
                    var memberGroup = await _context.UserGroups
                        .AnyAsync(g => g.UserId == currentUser && g.GroupId == groupId.Value);
                    if (!memberGroup)
                    {
                        result.ValidationMessage.Add("Nu ai permisiunea sa vezi evenimentele acestui grup");
                        return result;
                    }

                    var groupUserId = await _context.UserGroups
                        .Where(g => g.GroupId == groupId.Value)
                        .Select(g => g.UserId)
                        .ToListAsync();

                    events = await _context.Events
                        .Where(e =>
                            groupUserId.Contains(e.CreatedByUserId) &&
                            e.StartDate >= startDate &&
                            e.StartDate < endDate)
                        .Select(e => new EventModel
                        {
                            Id = e.Id,
                            Title = e.Title,
                            Description = e.Description,
                            StartDate = e.StartDate,
                            EndDate = e.EndDate,
                            Location = e.Location,
                            IsShared = e.IsShared,
                            ReminderMinutesBefore = e.ReminderMinutesBefore,
                            CreatedByUserId = e.CreatedByUserId
                        })
                        .ToListAsync();
                }
                else
                {
                    // Export evenimente personale
                    events = await _context.Events
                        .Where(e =>
                            e.CreatedByUserId == currentUser &&
                            e.StartDate >= startDate &&
                            e.StartDate < endDate)
                        .Select(e => new EventModel
                        {
                            Id = e.Id,
                            Title = e.Title,
                            Description = e.Description,
                            StartDate = e.StartDate,
                            EndDate = e.EndDate,
                            Location = e.Location,
                            IsShared = e.IsShared,
                            ReminderMinutesBefore = e.ReminderMinutesBefore,
                            CreatedByUserId = e.CreatedByUserId
                        })
                        .ToListAsync();
                }

                var icalContent = ICalendarService.ConvertEventsToICal(events);
                result.Result = icalContent;
            }
            catch (Exception ex)
            {
                result.ValidationMessage.Add($"An error occurred while exporting events: {ex.Message}");
            }

            return result;
        }

        public async Task<ServiceResult<List<EventModel>>> ImportEventsFromICal(string icalContent, int? groupId = null)
        {
            var result = new ServiceResult<List<EventModel>>();

            try
            {
                var currentUser = _currentUserService.UserId;
                if (currentUser == null)
                {
                    result.ValidationMessage.Add("User not authenticated");
                    return result;
                }

                // Parsează evenimentele din iCal
                var importedEvents = ICalendarService.ParseICalToEvents(icalContent);

                // Filtrează doar evenimentele din următoarele 30 de zile
                var startDate = DateTime.UtcNow.Date;
                var endDate = startDate.AddDays(30);

                var validEvents = importedEvents
                    .Where(e => e.StartDate >= startDate && e.StartDate < endDate)
                    .ToList();

                var createdEvents = new List<EventModel>();

                foreach (var eventModel in validEvents)
                {
                    // Setăm utilizatorul curent ca creator
                    eventModel.CreatedByUserId = (int)currentUser;

                    // Creează evenimentul în baza de date
                    var createResult = await CreateEvent(eventModel);
                    if (createResult.Result != null)
                    {
                        createdEvents.Add(createResult.Result);
                    }
                }

                result.Result = createdEvents;
            }
            catch (Exception ex)
            {
                result.ValidationMessage.Add($"An error occurred while importing events: {ex.Message}");
            }

            return result;
        }
    }
}
