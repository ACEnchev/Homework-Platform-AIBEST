namespace WebApplication14.Models
{
    public class Classes
    {
        public int ClassesId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int StudentCount { get; set; }

        public List<User>? users { get; set; }
        public List<Assignments>? assignments { get; set; }
    }
}
