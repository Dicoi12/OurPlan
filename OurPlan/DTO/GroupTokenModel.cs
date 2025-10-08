using AutoMapper;
using OurPlan.Entity;

namespace OurPlan.DTO
{
    public class GroupTokenModel
    {

        public int Id { get; set; }

        public int GroupId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public virtual Group? Group { get; set; }
    }
}