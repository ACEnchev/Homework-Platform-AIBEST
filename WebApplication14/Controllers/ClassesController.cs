
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication14.Data;
using WebApplication14.Models;

namespace WebApplication14.Controllers
{
    public class ClassesController : Controller
    {
        private readonly AppDbContext _context;
        public ClassesController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("classes")]
        public IActionResult GetAll()
        {
            return View(_context.classes.ToList());
        }
        [HttpGet]
        [Route("classes/{id}")]

        public IActionResult GetById([FromRoute] int id)
        {
            var classes = _context.classes.FirstOrDefault(x => x.ClassesId == id);
            return View(classes);
        }
        [HttpGet]
        [Route("classes/{id}/students")]
        public async Task<IActionResult> GetStudents([FromRoute] int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role == "Teacher")
            {
                var students = await _context.users
              .Where(x => x.Role == "Student")
              .Include(s => s.classes)
              .AsNoTracking()
              .Where(x => x.classes.ClassesId == id)
              .ToListAsync();
                return View(students);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }
        [HttpGet]
        [Route("classes/{id}/teachers")]
        public async Task<IActionResult> GetTeachers([FromRoute] int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role == "Admin")
            {
                var students = await _context.users
              .Where(x => x.Role == "Teacher")
              .Include(s => s.classes)
              .AsNoTracking()
              .Where(x => x.classes.ClassesId == id)
              .ToListAsync();
                return View(students);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        [Route("classes/{id}/assign-student")]
        public IActionResult AssignStudent()
        {
            var role = HttpContext.Session.GetString("Role");
            if(role == "Teacher" || role == "Admin")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            
        }
        [HttpPost]
        [Route("classes/{id}/assign-student")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignStudent([Bind("UserId")] User user, [FromRoute] int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role == "Teacher" || role == "Admin")
            {
                var existinguser = await _context.users.FirstOrDefaultAsync(x => x.UserId == user.UserId);
                if (existinguser != null)
                {
                    existinguser.ClassesId = id;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View();
                }
            }

            else
            {
                return View();
            }
            
        }
        [HttpGet]
        [Route("classes/{id}/assign-teacher")]
        public IActionResult AssignTeacher()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role == "Admin")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }
        [HttpPost]
        [Route("classes/{id}/assign-teacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTeacher([Bind("UserId")] User user, [FromRoute] int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role == "Admin")
            {
                var existinguser = await _context.users.FirstOrDefaultAsync(x => x.UserId == user.UserId);
                if (existinguser != null)
                {
                    existinguser.ClassesId = id;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
            
        }
        [HttpGet]
        [Route("classes/{id}/getusers")]
        public async Task<IActionResult> GetStudentsFromClass([FromRoute] int id)
        {
            var UsersInClass = await _context.student
                .Include(x => x.user)
                .AsNoTracking()
                .Where(x => x.user.ClassesId == id)
                .ToListAsync();
            return View(UsersInClass);
        }
    }
}
