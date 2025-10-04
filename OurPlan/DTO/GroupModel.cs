using OurPlan.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OurPlan.DTO
{
    public class GroupModel
    {
        
        public int Id { get; set; }

        public string Name { get; set; } = "Couple";
        
        public User? CreatedBy { get; set; }
        
        public int CreatedByUserId { get; set; }

        public ICollection<UserGroup> UserGropus { get; set; } = new List<UserGroup>();






    }
}

