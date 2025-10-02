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
        public readonly IUserService _userService;
        public readonly IMapper _mapper;
        public EventService(ApplicationDbContext context, IUserService userService, IMapper mapper)
        {
            _context = context;
            _userService = userService;
            _mapper = mapper;
        }
        public async Task<ServiceResult<EventModel>> CreateEvent(EventModel model)
        {
            var result = new ServiceResult<EventModel>();
            try
            {
                var currentUser = _userService.GetCurrentUser();
                if (currentUser == null)
                {
                    result.ValidationMessage.Add("User not authenticated");
                    return result;
                }

                model.CreatedByUserId = currentUser.Id;
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
                var currentUser = _userService.GetCurrentUser();
                if (currentUser == null)
                {
                    result.ValidationMessage.Add("User not authenticated");
                    return result;
                }
                var events = await _context.Events.Include(x => x.User)
                    .Where(e => e.CreatedByUserId == currentUser.Id)
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
