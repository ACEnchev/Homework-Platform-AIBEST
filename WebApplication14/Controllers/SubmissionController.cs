
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.ProjectModel;
using WebApplication14.Data;
using WebApplication14.Models;

namespace WebApplication14.Controllers
{
    public class SubmissionController : Controller
    {
        private readonly AppDbContext _context;
        public SubmissionController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("submission/view")]
        public IActionResult ViewSubmission()
        {
            var email = HttpContext.Session.GetString("Email");
            var password = HttpContext.Session.GetString("Password");

            var role = HttpContext.Session.GetString("Role");

            var user = _context.users.FirstOrDefault(x => x.Email == email && x.Password == password);

            if(role == "Teacher" || role == "Admin")
            {
                var ClassesID = user.ClassesId;

                var students = _context.submission
                    .Include(x => x.assignments)
                    .AsNoTracking()
                    .Where(x => x.assignments.ClassesId == ClassesID)
                    .ToList();
                    
                return View(students);
                
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet]
        [Route("submission/submit-homework/{id}")]
        public IActionResult Submit()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("submission/submit-homework/{id}")]

        public async Task<IActionResult> Submit(IFormFile file, [FromRoute] int id)
        {
            
            if (file != null)
            {
                if (file.Length > 0)
                {
                    var email = HttpContext.Session.GetString("Email");
                    var password = HttpContext.Session.GetString("Password");
                    var user = await _context.users.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);

                    var existingstudent = await _context.student.FirstOrDefaultAsync(x => x.UserId == user.UserId);
                    var fileName = Path.GetFileName(file.FileName);
                    var fileExtension = Path.GetExtension(fileName);
                    var newFileName = string.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);
                    var newFile = new Submission();
                    newFile.AssignmentsId = id;
                    newFile.StudentId = existingstudent.StudentId;
                    newFile.SubmissionDate = DateTime.Now;
                    newFile.Feedback = "No";
                    

                    using (var memory = new MemoryStream())
                    {
                        await file.CopyToAsync(memory);
                        newFile.Content = memory.ToArray();
                    }
                    _context.submission.Add(newFile);
                    
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }
        [HttpGet]
        [Route("submission/download/{id}")]
        public async Task<IActionResult> Download([FromRoute] int id)
        {
            var file = await _context.submission.FirstOrDefaultAsync(x => x.SubmissionId == id);
            if(file == null)
            {
                return NotFound();
            }
           
            return File(file.Content, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Homework.docx");
            
        }
        [HttpGet]
        [Route("submission/view/student/{id}")]

        public async Task<IActionResult> SeeSubmissions([FromRoute] int id)
        {
            var role = HttpContext.Session.GetString("Role");

            if(role == "Student")
            {
                var Estudent = await _context.student.FirstOrDefaultAsync(x => x.UserId == id);
                var SId = Estudent.StudentId;
                var subm = await _context.submission.Include(x => x.student)
                    .AsNoTracking().Where(x => x.student.StudentId == SId)
                    .ToListAsync();
                return View(subm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            
            
        }
    }
}
