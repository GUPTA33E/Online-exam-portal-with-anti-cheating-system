using System.ComponentModel.DataAnnotations;

namespace OnlineExamPortal.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(150), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Student"; // Admin or Student

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<Result> Results { get; set; } = new List<Result>();
        public ICollection<CheatLog> CheatLogs { get; set; } = new List<CheatLog>();
    }
}
