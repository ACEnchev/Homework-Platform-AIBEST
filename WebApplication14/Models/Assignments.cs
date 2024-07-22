namespace WebApplication14.Models
{
    public class Assignments
    {
        public int AssignmentsId { get; set; }
        public int? ClassesId { get; set; }
        public string Title { get; set; } = string.Empty;

        public byte[] Description { get; set; }

        public string DueData { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;

        public Classes? classes { get; set; }

        public List<Submission>? submission { get; set; }
    }
}
