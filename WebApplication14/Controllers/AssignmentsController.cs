using Microsoft.AspNetCore.Mvc;
using WebApplication14.Data;
using WebApplication14.Models;
using System.IO.MemoryMappedFiles;
using Microsoft.CodeAnalysis;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace WebApplication14.Controllers
{
    public class AssignmentsController : Controller
    {
        private readonly AppDbContext _context;

        public AssignmentsController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("assignments")]
        public IActionResult Index()
        {
            var files = _context.assignments.ToList();
            return View(files);
        }
        [HttpGet]
        [Route("assignments/upload")]
        public async Task<IActionResult> Upload()
        {
            var role = HttpContext.Session.GetString("Role");
            if(role == "Teacher" || role == "Admin")
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("assignments/upload")]
        public async Task<IActionResult> Upload([Bind("AssignmentsId,ClassesId,Title,Description,DueData,CreatedBy")] Assignments assignments, 
            IFormFile file)
        {
            if(file != null)
            {
                if(file.Length > 0)
                {
                    var email = HttpContext.Session.GetString("Email");
                    var fileName = Path.GetFileName(file.FileName);
                    var fileExtension = Path.GetExtension(fileName);
                    var newFileName = string.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);
                    var newFile = assignments;
                    newFile.CreatedBy = email;
                    newFile.Title = newFile.Title+fileExtension;

                    using (var memory = new MemoryStream())
                    {
                        await file.CopyToAsync(memory);
                        newFile.Description = memory.ToArray();
                    }
                    _context.assignments.Add(newFile);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
            
            
        }
        [HttpGet]
        [Route("assignments/view")]
        public async Task<IActionResult> ViewClasses()
        {
            var email = HttpContext.Session.GetString("Email");
            var password = HttpContext.Session.GetString("Password");
            var role = HttpContext.Session.GetString("Role");
            var user = await _context.users.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
            if(user != null)
            {
                var classId = user.ClassesId;
                var homework = await _context.assignments
             
             .Include(s => s.classes)
             .AsNoTracking()
             .Where(x => x.classes.ClassesId == classId)
             .ToListAsync();
                if(homework.Count == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.Role = role; 
                return View(homework);
            }
            else
            {
                return View();
            }
            
        }
        [HttpGet]
        [Route("assignments/download/{id}")]
        public async Task<IActionResult> Download([FromRoute] int id)
        {
            var file = await _context.assignments.FirstOrDefaultAsync(x => x.AssignmentsId == id);
            if(file == null)
            {
                return NotFound();
            }
            return File(file.Description, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", file.Title);

        }
        [HttpGet]
        [Route("assignments/delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var role = HttpContext.Session.GetString("Role");
            var assign = await _context.assignments.FirstOrDefaultAsync(x => x.AssignmentsId == id);
            if(assign != null && role == "Teacher")
            {
                _context.assignments.Remove(assign);
                await _context.SaveChangesAsync();
                
                return RedirectToAction("ViewClasses", "Assignments");
            }
            return RedirectToAction("Index", "Home");
        }
        
    }
}
