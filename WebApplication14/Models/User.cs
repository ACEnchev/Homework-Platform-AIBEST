namespace WebApplication14.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public int? ClassesId { get; set; }

        public Classes? classes { get; set; }
        public Student? student { get; set; }
        public Teacher? teacher { get; set; }
        public Administrator? administrator { get; set; }

    }
}
