using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var email = HttpContext.Session.GetString("Email");
            var uid = HttpContext.Session.GetInt32("UserId");
            var password = HttpContext.Session.GetString("Password");
            var role = HttpContext.Session.GetString("Role");
            if(role == "Teacher")
            {
                return View(await _context.users.ToListAsync());
            }
            else
            {
                
                return RedirectToAction("Index", "Home");
            }
        }
        public  User GetCurrentUser()
        {
           
            var uid = HttpContext.Session.GetInt32("UserId");
            var user = _context.users.FirstOrDefault(x => x.UserId == uid);
            return user;
        }
    }
}
