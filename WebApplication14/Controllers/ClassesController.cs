
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
            var role = HttpContext.Session.GetString("Role");
            if(role == "Admin")
            {
                return View(_context.classes.ToList());
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        [Route("classes/{id}")]

        public IActionResult GetById([FromRoute] int id)
        {
            var classes = _context.classes.FirstOrDefault(x => x.ClassesId == id);
            return View(classes);
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
                if (existinguser != null && existinguser.Role == "Student")
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
        [Route("classes/{id}/remove-student")]

        public async Task<IActionResult> RemoveStudent([FromRoute] int id)
        {
            var role = HttpContext.Session.GetString("Role");
            var user = await _context.users.FirstOrDefaultAsync(x => x.UserId == id);
            if (user != null && user.Role == "Student" && role == "Admin" || role == "Teacher")
            {
                user.ClassesId = 1;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");

        }
        [HttpGet]
        [Route("classes/{id}/remove-teacher")]

        public async Task<IActionResult> RemoveTeacher([FromRoute] int id)
        {
            var role = HttpContext.Session.GetString("Role");
            var user = await _context.users.FirstOrDefaultAsync(x => x.UserId == id);
            if(user != null && user.Role == "Teacher" && role == "Admin")
            {
                user.ClassesId = 1;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
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
            var role = HttpContext.Session.GetString("Role");
            if(role == "Teacher" || role == "Admin")
            {
                var UsersInClass = await _context.student
                .Include(x => x.user)
                .AsNoTracking()
                .Where(x => x.user.ClassesId == id)
                .ToListAsync();
                return View(UsersInClass);
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        [Route("classes/{id}/getteachers")]

        public async Task<IActionResult> GetTeachersFromClass([FromRoute] int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if(role == "Admin")
            {
                var teachersInClass = await _context.teachers
                    .Include(x => x.user)
                    .AsNoTracking()
                    .Where(x => x.user.ClassesId == id)
                    .ToListAsync();
                return View(teachersInClass);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
