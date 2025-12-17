using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurPlan.Entity
{
  
    
    public class GroupTask
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        
        [StringLength(1000)]
        
        public string? Description { get; set; } 
        
        public TaskPriority Priority { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public string? DueDate { get; set; }
        
        
        [ForeignKey(nameof(Group))]
        public int GroupId { get; set; }
        
        [ForeignKey(nameof(User))]
        public int CreatedByUserId { get; set; }
        
        public virtual User? User { get; set; }
        
        
        
        
    }
}