using OurPlan.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OurPlan.DTO
{
    public class EventModel
    {
        
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? Location { get; set; }

     
        public int CreatedByUserId { get; set; }
        public virtual User? User { get; set; }

        public bool IsShared { get; set; } = true;

        public int? ReminderMinutesBefore { get; set; }
    }
}
