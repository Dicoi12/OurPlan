using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurPlan.Entity
{
    public enum TaskPriority
    {
        Low = 1,
        Medium = 2,
        High = 3
    }
    
    public class GroupTask
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
        
        
        [ForeignKey("GroupId")]
        public int GroupId { get; set; }
        
        [ForeignKey("CreatedByUserId")]
        public int CreatedByUserId { get; set; }
        
        
        
        
    }
}