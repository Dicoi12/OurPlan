using OurPlan.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OurPlan.DTO
{
    public class GroupModel
    {
        
        public int Id { get; set; }

        public string Name { get; set; } = "Couple";
        
        public UserModel? CreatedBy { get; set; }
        
        public int CreatedByUserId { get; set; }

        public ICollection<UserGroup> UserGropus { get; set; } = new List<UserGroup>();






    }
}

