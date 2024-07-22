namespace WebApplication14.Models
{
    public class Teacher
    {
        public int TeacherId { get; set; }
        public int? UserId { get; set; }
        public User? user { get; set; }
    }
}
