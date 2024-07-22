namespace WebApplication14.Models
{
    public class Administrator
    {
        public int AdministratorId { get; set; }
        public int? UserId { get; set; }

        public User? user { get; set; }
    }
}
