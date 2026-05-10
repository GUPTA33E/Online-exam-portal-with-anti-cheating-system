using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineExamPortal.Data;
using OnlineExamPortal.Models;
using OnlineExamPortal.Models.ViewModels;
using System.Security.Claims;
using System.Text.Json;

namespace OnlineExamPortal.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // ─── Dashboard ───────────────────────────────────────────────────────────
        public async Task<IActionResult> Dashboard()
        {
            int userId = GetUserId();

            var exams = await _context.Exams
                .Where(e => e.IsActive)
                .Include(e => e.Questions)
                .ToListAsync();

            var attempted = await _context.Results
                .Where(r => r.UserId == userId)
                .Select(r => r.ExamId)
                .ToListAsync();

            ViewBag.AttemptedExamIds = attempted;
            return View(exams);
        }

        // ─── Start Exam ──────────────────────────────────────────────────────────
        public async Task<IActionResult> StartExam(int examId)
        {
            int userId = GetUserId();

            // Check already attempted
            var existing = await _context.Results
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ExamId == examId);

            if (existing != null)
            {
                TempData["Warning"] = "You have already attempted this exam.";
                return RedirectToAction("MyResult", new { resultId = existing.Id });
            }

            var exam = await _context.Exams
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == examId && e.IsActive);

            if (exam == null || !exam.Questions.Any())
            {
                TempData["Error"] = "Exam not found or has no questions.";
                return RedirectToAction("Dashboard");
            }

            var vm = new ExamAttemptViewModel
            {
                Exam = exam,
                Questions = exam.Questions.OrderBy(q => q.Id).ToList()
            };

            return View(vm);
        }

        // ─── Submit Exam ─────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> SubmitExam(int examId, Dictionary<int, string> answers,
            int cheatWarnings = 0, bool autoSubmitted = false)
        {
            int userId = GetUserId();

            // Prevent double submission
            if (await _context.Results.AnyAsync(r => r.UserId == userId && r.ExamId == examId))
                return RedirectToAction("Dashboard");

            var questions = await _context.Questions
                .Where(q => q.ExamId == examId)
                .ToListAsync();

            int score = 0;
            int totalMarks = questions.Sum(q => q.Marks);
            int correctCount = 0;

            foreach (var q in questions)
            {
                if (answers.TryGetValue(q.Id, out var selected) &&
                    selected.Equals(q.CorrectAnswer, StringComparison.OrdinalIgnoreCase))
                {
                    score += q.Marks;
                    correctCount++;
                }
            }

            double percentage = totalMarks > 0 ? Math.Round((double)score / totalMarks * 100, 2) : 0;

            var result = new Result
            {
                UserId        = userId,
                ExamId        = examId,
                Score         = score,
                TotalMarks    = totalMarks,
                Percentage    = percentage,
                IsPassed      = percentage >= 40,
                Answers       = JsonSerializer.Serialize(answers),
                CheatWarnings = cheatWarnings,
                AutoSubmitted = autoSubmitted
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyResult", new { resultId = result.Id });
        }

        // ─── Log Cheat ───────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> LogCheat(int examId, string violationType)
        {
            int userId = GetUserId();
            _context.CheatLogs.Add(new CheatLog
            {
                UserId        = userId,
                ExamId        = examId,
                ViolationType = violationType
            });
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // ─── My Result ───────────────────────────────────────────────────────────
        public async Task<IActionResult> MyResult(int resultId)
        {
            int userId = GetUserId();

            var result = await _context.Results
                .Include(r => r.Exam)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == resultId && r.UserId == userId);

            if (result == null) return NotFound();

            var questions = await _context.Questions
                .Where(q => q.ExamId == result.ExamId)
                .ToListAsync();

            var userAnswers = string.IsNullOrEmpty(result.Answers)
                ? new Dictionary<int, string>()
                : JsonSerializer.Deserialize<Dictionary<int, string>>(result.Answers) ?? new();

            string grade = result.Percentage >= 90 ? "A+" :
                           result.Percentage >= 80 ? "A"  :
                           result.Percentage >= 70 ? "B"  :
                           result.Percentage >= 60 ? "C"  :
                           result.Percentage >= 40 ? "D"  : "F";

            var vm = new ResultViewModel
            {
                Result         = result,
                Exam           = result.Exam!,
                Student        = result.User!,
                TotalQuestions = questions.Count,
                CorrectAnswers = questions.Count(q =>
                    userAnswers.TryGetValue(q.Id, out var a) &&
                    a.Equals(q.CorrectAnswer, StringComparison.OrdinalIgnoreCase)),
                Grade = grade
            };

            ViewBag.Questions   = questions;
            ViewBag.UserAnswers = userAnswers;

            return View(vm);
        }

        // ─── My Results History ──────────────────────────────────────────────────
        public async Task<IActionResult> MyResults()
        {
            int userId = GetUserId();
            var results = await _context.Results
                .Include(r => r.Exam)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.SubmittedAt)
                .ToListAsync();
            return View(results);
        }
    }
}
