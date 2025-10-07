using OurPlan.Entity.Audit;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurPlan.Entity
{
    public class GroupToken : BaseAuditEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey(nameof(Group))]
        public int GroupId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public virtual Group? Group { get; set; }
    }
}
