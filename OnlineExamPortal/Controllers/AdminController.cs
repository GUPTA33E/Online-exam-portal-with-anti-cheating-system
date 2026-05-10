using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineExamPortal.Data;
using OnlineExamPortal.Models;
using OnlineExamPortal.Models.ViewModels;

namespace OnlineExamPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ─── Dashboard ───────────────────────────────────────────────────────────
        public async Task<IActionResult> Dashboard()
        {
            var vm = new AdminDashboardViewModel
            {
                TotalStudents = await _context.Users.CountAsync(u => u.Role == "Student"),
                TotalExams    = await _context.Exams.CountAsync(),
                TotalAttempts = await _context.Results.CountAsync(),
                AverageScore  = await _context.Results.AnyAsync()
                    ? Math.Round(await _context.Results.AverageAsync(r => r.Percentage), 2) : 0,
                RecentExams   = await _context.Exams.OrderByDescending(e => e.CreatedAt).Take(5).ToListAsync(),
                RecentResults = await _context.Results
                    .Include(r => r.User).Include(r => r.Exam)
                    .OrderByDescending(r => r.SubmittedAt).Take(10).ToListAsync()
            };
            return View(vm);
        }

        // ─── Exams ───────────────────────────────────────────────────────────────
        public async Task<IActionResult> Exams()
        {
            var exams = await _context.Exams
                .Include(e => e.Questions)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
            return View(exams);
        }

        [HttpGet]
        public IActionResult CreateExam() => View(new ExamCreateViewModel());

        [HttpPost]
        public async Task<IActionResult> CreateExam(ExamCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var exam = new Exam
            {
                Title = model.Title,
                Description = model.Description,
                DurationMinutes = model.DurationMinutes,
                IsActive = model.IsActive
            };
            _context.Exams.Add(exam);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Exam created successfully!";
            return RedirectToAction("ManageQuestions", new { examId = exam.Id });
        }

        [HttpGet]
        public async Task<IActionResult> EditExam(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam == null) return NotFound();

            return View(new ExamCreateViewModel
            {
                Id = exam.Id,
                Title = exam.Title,
                Description = exam.Description,
                DurationMinutes = exam.DurationMinutes,
                IsActive = exam.IsActive
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditExam(ExamCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var exam = await _context.Exams.FindAsync(model.Id);
            if (exam == null) return NotFound();

            exam.Title = model.Title;
            exam.Description = model.Description;
            exam.DurationMinutes = model.DurationMinutes;
            exam.IsActive = model.IsActive;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Exam updated successfully!";
            return RedirectToAction("Exams");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteExam(int id)
        {
            var exam = await _context.Exams
                .Include(e => e.Questions)
                .Include(e => e.Results)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exam == null) return NotFound();

            _context.Questions.RemoveRange(exam.Questions);
            _context.Results.RemoveRange(exam.Results);
            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Exam deleted successfully!";
            return RedirectToAction("Exams");
        }

        // ─── Questions ───────────────────────────────────────────────────────────
        public async Task<IActionResult> ManageQuestions(int examId)
        {
            var exam = await _context.Exams
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == examId);

            if (exam == null) return NotFound();

            ViewBag.Exam = exam;
            return View(exam.Questions.ToList());
        }

        [HttpGet]
        public IActionResult AddQuestion(int examId)
        {
            return View(new QuestionViewModel { ExamId = examId });
        }

        [HttpPost]
        public async Task<IActionResult> AddQuestion(QuestionViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var question = new Question
            {
                ExamId       = model.ExamId,
                QuestionText = model.QuestionText,
                OptionA      = model.OptionA,
                OptionB      = model.OptionB,
                OptionC      = model.OptionC,
                OptionD      = model.OptionD,
                CorrectAnswer = model.CorrectAnswer,
                Marks        = model.Marks
            };
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Question added!";
            return RedirectToAction("ManageQuestions", new { examId = model.ExamId });
        }

        [HttpGet]
        public async Task<IActionResult> EditQuestion(int id)
        {
            var q = await _context.Questions.FindAsync(id);
            if (q == null) return NotFound();

            return View(new QuestionViewModel
            {
                Id = q.Id, ExamId = q.ExamId, QuestionText = q.QuestionText,
                OptionA = q.OptionA, OptionB = q.OptionB,
                OptionC = q.OptionC, OptionD = q.OptionD,
                CorrectAnswer = q.CorrectAnswer, Marks = q.Marks
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditQuestion(QuestionViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var q = await _context.Questions.FindAsync(model.Id);
            if (q == null) return NotFound();

            q.QuestionText  = model.QuestionText;
            q.OptionA       = model.OptionA;
            q.OptionB       = model.OptionB;
            q.OptionC       = model.OptionC;
            q.OptionD       = model.OptionD;
            q.CorrectAnswer = model.CorrectAnswer;
            q.Marks         = model.Marks;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Question updated!";
            return RedirectToAction("ManageQuestions", new { examId = q.ExamId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var q = await _context.Questions.FindAsync(id);
            if (q == null) return NotFound();

            int examId = q.ExamId;
            _context.Questions.Remove(q);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Question deleted!";
            return RedirectToAction("ManageQuestions", new { examId });
        }

        // ─── Results ─────────────────────────────────────────────────────────────
        public async Task<IActionResult> Results(int? examId)
        {
            var query = _context.Results
                .Include(r => r.User)
                .Include(r => r.Exam)
                .AsQueryable();

            if (examId.HasValue)
                query = query.Where(r => r.ExamId == examId.Value);

            var results = await query.OrderByDescending(r => r.SubmittedAt).ToListAsync();
            ViewBag.Exams = await _context.Exams.ToListAsync();
            ViewBag.SelectedExamId = examId;
            return View(results);
        }

        // ─── Students ────────────────────────────────────────────────────────────
        public async Task<IActionResult> Students()
        {
            var students = await _context.Users
                .Where(u => u.Role == "Student")
                .OrderBy(u => u.Name)
                .ToListAsync();
            return View(students);
        }

        // ─── Cheat Logs ──────────────────────────────────────────────────────────
        public async Task<IActionResult> CheatLogs()
        {
            var logs = await _context.CheatLogs
                .Include(c => c.User)
                .Include(c => c.Exam)
                .OrderByDescending(c => c.LoggedAt)
                .Take(100)
                .ToListAsync();
            return View(logs);
        }
    }
}
