using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineExamPortal.Models
{
    public class CheatLog
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Exam")]
        public int ExamId { get; set; }

        public string ViolationType { get; set; } = string.Empty; // TabSwitch, RightClick, CopyPaste, FullScreenExit

        public DateTime LoggedAt { get; set; } = DateTime.Now;

        // Navigation
        public User? User { get; set; }
        public Exam? Exam { get; set; }
    }
}
