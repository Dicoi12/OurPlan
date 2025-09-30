using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurPlan.Entity
{
    public class UserGroup
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public virtual User? User { get; set; }

        [ForeignKey(nameof(Group))]
        public int GroupId { get; set; }
        public virtual Group? Group { get; set; }

        public string Role { get; set; } = "Member"; // eventual: Admin / Member
    }
}
