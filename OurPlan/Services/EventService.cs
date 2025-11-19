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

        public async Task<ServiceResult<List<EventModel>>> GetEventsForGroup(int groupId)
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
                    
                    
                    
                var utcNow = DateTime.UtcNow;
                var startOfDay = utcNow.Date;
                var endOfDay = startOfDay.AddDays(1);
                
                

                var events = await _context.Events
                    .Where(e =>
                        groupUserId.Contains(e.CreatedByUserId) &&
                        (
                            (e.StartDate >= startOfDay && e.StartDate < endOfDay) ||
                            (e.EndDate >= startOfDay && e.EndDate < endOfDay) ||
                            (e.StartDate <= startOfDay && e.EndDate >= endOfDay)
                        )
                    )
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

                result.Result = events;
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
            var entity = _context.Events.FirstOrDefault(e => e.Id == eventId);
            if (entity == null)
            {
                result.ValidationMessage.Add("Event not found");
                return result;
            }

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
                        User = e.User,
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
    }
}
