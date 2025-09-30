using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurPlan.Entity
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? Location { get; set; }

        [ForeignKey(nameof(User))]
        public int CreatedByUserId { get; set; }
        public virtual User? User { get; set; }

        public bool IsShared { get; set; } = true;
        
        public int? ReminderMinutesBefore { get; set; }
    }
}
