using Microsoft.EntityFrameworkCore;
using OnlineExamPortal.Models;

namespace OnlineExamPortal.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<CheatLog> CheatLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Suppress non-deterministic model warning (we control the seed data)
            optionsBuilder.ConfigureWarnings(w =>
                w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique email index
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // ── Seed: Admin user  (password: Admin@123)
            // Hash is pre-computed and static so EF model is stable
            const string adminHash = "$2a$11$K5nHR1REjlyNVbACLEpIf.c8mFT3WfRcmXrxmfn2nKPFjvvbkQIr6";
            modelBuilder.Entity<User>().HasData(new User
            {
                Id           = 1,
                Name         = "Admin",
                Email        = "admin@exam.com",
                PasswordHash = adminHash,
                Role         = "Admin",
                CreatedAt    = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            // ── Seed: Sample exam
            modelBuilder.Entity<Exam>().HasData(new Exam
            {
                Id              = 1,
                Title           = "C# Programming Basics",
                Description     = "Test your knowledge of C# fundamentals",
                DurationMinutes = 10,
                IsActive        = true,
                CreatedAt       = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            // ── Seed: 5 sample questions
            modelBuilder.Entity<Question>().HasData(
                new Question
                {
                    Id = 1, ExamId = 1,
                    QuestionText  = "What is the correct way to declare a variable in C#?",
                    OptionA = "var x = 5;",
                    OptionB = "variable x = 5;",
                    OptionC = "dim x = 5;",
                    OptionD = "let x = 5;",
                    CorrectAnswer = "A", Marks = 1
                },
                new Question
                {
                    Id = 2, ExamId = 1,
                    QuestionText  = "Which keyword is used to define a class in C#?",
                    OptionA = "struct",
                    OptionB = "define",
                    OptionC = "class",
                    OptionD = "type",
                    CorrectAnswer = "C", Marks = 1
                },
                new Question
                {
                    Id = 3, ExamId = 1,
                    QuestionText  = "What does 'OOP' stand for?",
                    OptionA = "Object Oriented Programming",
                    OptionB = "Open Object Process",
                    OptionC = "Oriented Object Pattern",
                    OptionD = "None of the above",
                    CorrectAnswer = "A", Marks = 1
                },
                new Question
                {
                    Id = 4, ExamId = 1,
                    QuestionText  = "Which of the following is a value type in C#?",
                    OptionA = "string",
                    OptionB = "object",
                    OptionC = "int",
                    OptionD = "array",
                    CorrectAnswer = "C", Marks = 1
                },
                new Question
                {
                    Id = 5, ExamId = 1,
                    QuestionText  = "What is the base class for all classes in C#?",
                    OptionA = "Base",
                    OptionB = "System",
                    OptionC = "Object",
                    OptionD = "Root",
                    CorrectAnswer = "C", Marks = 1
                }
            );
        }
    }
}
