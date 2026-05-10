using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineExamPortal.Models
{
    public class Question
    {
        public int Id { get; set; }

        [ForeignKey("Exam")]
        public int ExamId { get; set; }

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        [Required]
        public string OptionA { get; set; } = string.Empty;

        [Required]
        public string OptionB { get; set; } = string.Empty;

        [Required]
        public string OptionC { get; set; } = string.Empty;

        [Required]
        public string OptionD { get; set; } = string.Empty;

        [Required]
        public string CorrectAnswer { get; set; } = string.Empty; // "A", "B", "C", or "D"

        public int Marks { get; set; } = 1;

        // Navigation
        public Exam? Exam { get; set; }
    }
}
