using OurPlan.DTO;
using OurPlan.Entity;
using AutoMapper;


namespace OurPlan.Helpers
{
    public class GenericProfile : Profile
    {
        public GenericProfile()
        {
            CreateMap<Event, EventModel>().ReverseMap();
        }
    }
}
