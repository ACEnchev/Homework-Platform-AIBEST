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
    }
}
