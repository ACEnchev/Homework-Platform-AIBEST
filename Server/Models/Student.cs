namespace WebApplication14.Models
{
    public class Student
    {
        public int StudentId { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public int? UserId { get; set; }
        public User? user { get; set; }

        public List<Submission>? submission { get; set; }
    }
}
