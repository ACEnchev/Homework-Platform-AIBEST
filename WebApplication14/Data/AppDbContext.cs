using Microsoft.EntityFrameworkCore;
using WebApplication14.Models;

namespace WebApplication14.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options)
            : base(options)
        {
                
        }
        public DbSet<User> users { get; set; }
        public DbSet<Classes> classes { get; set; }

        public DbSet<Assignments> assignments { get; set; }

        public DbSet<Student> student { get; set; }
        public DbSet<Submission> submission { get; set; }
        public DbSet<Grades> grades { get; set; }

        public DbSet<Administrator> administrators { get; set; }
        public DbSet<Teacher> teachers { get; set; }

    }
}
