using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using WebApplication14.Data;
using WebApplication14.Models;
using System.Web;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication14.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("account/register")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Register([Bind("UserId,Email,Password,LastName,FirstName,Username")] User user,
            string phoneNumber)
        {
            if (ModelState.IsValid)
            {
                user.Role = "Student";
                user.ClassesId = 1;
                
                var existingUser = _context.users.FirstOrDefault(x => x.UserId == user.UserId && x.Email == user.Email && x.Password == user.Password);
                if(existingUser == null)
                {
                    
                    _context.users.Add(user);
                    
                    _context.SaveChanges();
                    Student student = new Student
                    {
                        UserId = user.UserId,
                        PhoneNumber = phoneNumber,

                    };
                    _context.student.Add(student);
                    _context.SaveChanges();
                    return RedirectToAction("Login");
                }
            }
            return View();
        }
        [HttpGet]
        [Route("account/login")]

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("UserId,Email,Password")] User user           )
        {
            var loggedUser = _context.users.FirstOrDefault(x => x.Password == user.Password && x.Email == user.Email);
            if(loggedUser != null)
            {
                
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));


                HttpContext.Session.SetString("Email", loggedUser.Email);
                HttpContext.Session.SetInt32("UserId", loggedUser.UserId);
                HttpContext.Session.SetString("Password", loggedUser.Password);
                HttpContext.Session.SetString("Role", loggedUser.Role);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Account");
        }

        public User GetCurrentUser()
        {

            var uid = HttpContext.Session.GetInt32("UserId");
            var user = _context.users.FirstOrDefault(x => x.UserId == uid);
            return user;
        }
    }
}
