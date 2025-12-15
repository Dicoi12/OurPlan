using OurPlan.Entity;
using System.ComponentModel.DataAnnotations;

namespace OurPlan.DTO
{
    public class GroupTaskModel
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        
        [StringLength(1000)]
        
        public string Description { get; set; } 
        
        public TaskPriority Priority { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public string? DueDate { get; set; }
        
        public int GroupId { get; set; }
        
        public int CreatedByUserId { get; set; }
        
        
    }
}