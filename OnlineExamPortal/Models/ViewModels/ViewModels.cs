using System.ComponentModel.DataAnnotations;

namespace OnlineExamPortal.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string Role { get; set; } = "Student";
    }

    public class ExamCreateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(5, 180, ErrorMessage = "Duration must be between 5 and 180 minutes")]
        public int DurationMinutes { get; set; } = 30;

        public bool IsActive { get; set; } = true;
    }

    public class QuestionViewModel
    {
        public int Id { get; set; }

        [Required]
        public int ExamId { get; set; }

        [Required(ErrorMessage = "Question text is required")]
        public string QuestionText { get; set; } = string.Empty;

        [Required(ErrorMessage = "Option A is required")]
        public string OptionA { get; set; } = string.Empty;

        [Required(ErrorMessage = "Option B is required")]
        public string OptionB { get; set; } = string.Empty;

        [Required(ErrorMessage = "Option C is required")]
        public string OptionC { get; set; } = string.Empty;

        [Required(ErrorMessage = "Option D is required")]
        public string OptionD { get; set; } = string.Empty;

        [Required(ErrorMessage = "Correct answer is required")]
        public string CorrectAnswer { get; set; } = string.Empty;

        public int Marks { get; set; } = 1;
    }

    public class ExamAttemptViewModel
    {
        public Exam Exam { get; set; } = new Exam();
        public List<Question> Questions { get; set; } = new List<Question>();
        public Dictionary<int, string> StudentAnswers { get; set; } = new Dictionary<int, string>();
    }

    public class ResultViewModel
    {
        public Result Result { get; set; } = new Result();
        public Exam Exam { get; set; } = new Exam();
        public User Student { get; set; } = new User();
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public string Grade { get; set; } = string.Empty;
    }

    public class AdminDashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int TotalExams { get; set; }
        public int TotalAttempts { get; set; }
        public double AverageScore { get; set; }
        public List<Exam> RecentExams { get; set; } = new List<Exam>();
        public List<Result> RecentResults { get; set; } = new List<Result>();
    }
}
