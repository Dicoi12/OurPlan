using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurPlan.Entity
{
    public class Group
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "Cuplu";

        [ForeignKey(nameof(CreatedBy))]
        public int? CreatedByUserId { get; set; }
        public User? CreatedBy { get; set; }

        public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();

    }
}
