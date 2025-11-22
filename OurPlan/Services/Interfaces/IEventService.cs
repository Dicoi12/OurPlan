using OurPlan.DTO;
using OurPlan.Helpers;

namespace OurPlan.Services.Interfaces
{
    public interface IEventService
    {
        public Task<ServiceResult<List<EventModel>>> GetEventsForCurrentUser();
        public Task<ServiceResult<EventModel>> CreateEvent(EventModel model);
        public Task<ServiceResult<EventModel>> UpdateEvent(EventModel model);
        public Task<ServiceResult<EventModel>> DeleteEvent(int eventId);
        
        public Task<ServiceResult<List<EventModel>>> GetEventsForGroup(int groupId, string viewMode, DateTime? date);

    }
}
