using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineExamPortal.Models
{
    public class Result
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Exam")]
        public int ExamId { get; set; }

        public int Score { get; set; }
        public int TotalMarks { get; set; }
        public double Percentage { get; set; }

        public bool IsPassed { get; set; }

        public string? Answers { get; set; } // JSON: {"QuestionId": "SelectedOption"}

        public int CheatWarnings { get; set; } = 0;

        public bool AutoSubmitted { get; set; } = false;

        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        // Navigation
        public User? User { get; set; }
        public Exam? Exam { get; set; }
    }
}
