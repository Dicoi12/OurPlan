using System.ComponentModel.DataAnnotations;

namespace OurPlan.Entity
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        // parola se va salva HASH-uită, nu în clar!
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
