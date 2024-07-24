using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebApplication14.Data;
using WebApplication14.Models;

namespace WebApplication14.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("users/getinfo/{id}")]

        public async Task<IActionResult> GetInfo([FromRoute] int id)
        {
            
            var email = HttpContext.Session.GetString("Email");
            var uid = HttpContext.Session.GetInt32("UserId");
            var password = HttpContext.Session.GetString("Password");
            var role = HttpContext.Session.GetString("Role");
            User user = new User()
            {
                Email = email,
                UserId = id,
                Password = password,
                Role = role
            };

           
            return View(user);
        }
        [HttpGet]
        [Route("users/getall")]
        public async Task<IActionResult> GetUsers()
        {
            
            var role = HttpContext.Session.GetString("Role");
            if(role == "Admin")
            {
                return View(await _context.users.ToListAsync());
            }
            else
            {
                
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet]
        [Route("users/delete/{id}")]

        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var user = await _context.users.FirstOrDefaultAsync(x => x.UserId == id);
            if(user != null)
            {
                FindAndRemoveTeacher(user);
                await _context.SaveChangesAsync();
                _context.users.Remove(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("GetUsers", "User");
            }
            return RedirectToAction("Index", "Home");
            
        }

        [HttpGet]
        [Route("users/getusers/{id}")]
        public IActionResult GetUsersFromClass([FromRoute] int id)
        {
            var studentsInClass = _context.users
                .Where(x => x.Role == "Student")
                .Include(x => x.classes)
                .Where(x => x.classes.ClassesId == id)
                .ToList();
            return View(studentsInClass);
        }
        
        [HttpGet]
        [Route("users/addrole/{id}")]
        public async Task<IActionResult> AddRole([FromRoute] int id)
        {
            var role = HttpContext.Session.GetString("Role");

            var users = await _context.users.FirstOrDefaultAsync(x => x.UserId == id);
            if(users != null && role == "Admin")
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [Route("users/addrole/{id}")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> AddRole([FromRoute] int id, string role)
        {
            var users = await _context.users.FirstOrDefaultAsync(x => x.UserId == id);
            if (users != null)
            {
                if (role == "Teacher")
                {
                    ChangeToTeacher(users);
                }
                else if (role == "Admin")
                {
                    ChangeToAdmin(users);
                }
                else if (role == "Student")
                {
                    ChangeToStudent(users);
                }
                
            }
            return RedirectToAction("Index", "Home");
        }
        public User GetCurrentUser()
        {
            var email = HttpContext.Session.GetString("Email");
            var password = HttpContext.Session.GetString("Password");
            var user = _context.users.FirstOrDefault(x => x.Email == email && x.Password == password);
            if(user != null)
            {
                return user;
            }
            else
            {
                return null;
            }
        }
        public void ChangeToTeacher(User user)
        {
            Teacher newTeacher = new Teacher();
           
            if(user.Role == "Student")
            {
                FindAndRemoveStudent(user);
                MakeTeacher(user, newTeacher);                
            }
            else if(user.Role == "Admin")
            {
                FindAndRemoveAdmin(user);
                MakeTeacher(user, newTeacher);
            }          
        }
        public void ChangeToAdmin(User user)
        {
            Administrator newAdmin = new Administrator();
            if(user.Role == "Student")
            {
                FindAndRemoveStudent(user);
                MakeAdmin(user, newAdmin);
            }
            else if(user.Role == "Teacher")
            {
                FindAndRemoveTeacher(user);
                MakeAdmin(user, newAdmin);
            }
        }
        public void ChangeToStudent(User user)
        {
            Student newStudent = new Student();
            if(user.Role == "Teacher")
            {
                FindAndRemoveTeacher(user);
                MakeStudent(user, newStudent);
            }
            else if(user.Role == "Admin")
            {
                FindAndRemoveAdmin(user);
                MakeStudent(user, newStudent);
            }
        }
        public void FindAndRemoveStudent(User user)
        {
            var FindStudent = _context.student.FirstOrDefault(x => x.UserId == user.UserId);
            if (FindStudent != null)
            {
                _context.student.Remove(FindStudent);
                _context.SaveChanges();
            }
        }
        public void FindAndRemoveAdmin(User user)
        {
            var FindAdmin = _context.administrators.FirstOrDefault(x => x.UserId == user.UserId);
            if(FindAdmin != null)
            {
                _context.administrators.Remove(FindAdmin);
                _context.SaveChanges();
            }
        }
        public void FindAndRemoveTeacher(User user)
        {
            var FindTeacher = _context.teachers.FirstOrDefault(x => x.UserId == user.UserId);
            if(FindTeacher != null)
            {
                _context.teachers.Remove(FindTeacher);
                _context.SaveChanges();
            }
        }
        public void MakeTeacher(User user, Teacher teacher)
        {
            user.Role = "Teacher";
            teacher.UserId= user.UserId;
            _context.teachers.Add(teacher);
            _context.SaveChanges();
        }
        public void MakeAdmin(User user, Administrator admin)
        {
            user.Role = "Admin";
            admin.UserId= user.UserId;
            _context.administrators.Add(admin);
            _context.SaveChanges();
        }
        public void MakeStudent(User user, Student student)
        {
            user.Role = "Student";
            student.UserId= user.UserId;
            _context.student.Add(student);
            _context.SaveChanges();
        }
    }
}
