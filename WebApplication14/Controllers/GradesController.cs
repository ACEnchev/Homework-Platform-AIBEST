using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication14.Data;
using WebApplication14.Models;

namespace WebApplication14.Controllers
{
    public class GradesController : Controller
    {
        private readonly AppDbContext _context;

        public GradesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("grades/add/{id}")]
        public IActionResult AddGrade()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role == "Teacher" || role == "Admin")
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [Route("grades/add/{id}")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> AddGrade([Bind("GradesId,SubmissionId,Grade,GradedBy")] Grades grades
            , [FromRoute] int id,  string feedback)
        {
            var email = HttpContext.Session.GetString("Email");
            var homework = await _context.submission.FirstOrDefaultAsync(x => x.SubmissionId == id);
            if (homework != null)
            {
                grades.SubmissionId = id;
                grades.GradedBy = email;
                homework.Feedback = feedback;

                await _context.grades.AddAsync(grades);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpGet]
        [Route("grades/seegrade/{id}")]

       public async Task<IActionResult> SeeGrade([FromRoute] int id)
        {
            var existingGrade = await _context.grades.FirstOrDefaultAsync(x => x.SubmissionId == id);
            if (existingGrade != null)
            {
                return View(existingGrade);
            }
            return RedirectToAction("Index", "Home");

        }
        [HttpGet]
        [Route("grades/student/{id}")]
        public async Task<IActionResult> CheckGrade([FromRoute] int id)
        {
            var role = HttpContext.Session.GetString("Role");
            var email = HttpContext.Session.GetString("Email");
            var password = HttpContext.Session.GetString("Password");
            var user = _context.users.FirstOrDefault(x => x.Email == email && x.Password == password);

            

            if(role == "Teacher" && user != null || role == "Admin")
            {
                var StudentGrades = await _context.grades
               .Include(x => x.submission)
               .AsNoTracking()
               .Where(x => x.submission.StudentId == id)
               .ToListAsync();
                return View(StudentGrades);
            }
            else if(role == "Student" && user != null)
            {
                var student = _context.student.FirstOrDefault(x => x.UserId == user.UserId);

                var stuId = student.StudentId;
                if(id != stuId)
                {
                    return RedirectToAction("Index", "Home");
                }
                var StudentGrades = await _context.grades
               .Include(x => x.submission)
               .AsNoTracking()
               .Where(x => x.submission.StudentId == id)
               .ToListAsync();
                return View(StudentGrades);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
