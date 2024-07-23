
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApplication14.Data;
using WebApplication14.Models;

namespace WebApplication14.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        public HomeController(ILogger<HomeController> logger
            , AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var email = HttpContext.Session.GetString("Email");
             var id = HttpContext.Session.GetInt32("UserId");
            var password = HttpContext.Session.GetString("Password");
            var role = HttpContext.Session.GetString("Role");
            
            var user = _context.users.FirstOrDefault(x =>x.UserId == id);
            

            if (email != null && id != null && password != null && user != null)
            {
                var classe = _context.classes.FirstOrDefault(x => x.ClassesId == user.ClassesId);
                
                ViewBag.Email = email;
                ViewBag.UserId = id;
                ViewBag.Passowrd = password;
                ViewBag.Role = role;
                ViewBag.ClassId = user.ClassesId;
                ViewBag.ClassName = classe.ClassName;
                if(role == "Student")
                {
                    var student = _context.student.FirstOrDefault(x => x.UserId == user.UserId);
                    ViewBag.StudentId = student.StudentId;
                }
                

                return View("~/Views/Home/IndexTest.cshtml");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public User GetCurrentUser()
        {

            var uid = HttpContext.Session.GetInt32("UserId");
            var user = _context.users.FirstOrDefault(x => x.UserId == uid);
            return user;
        }
    }
}
