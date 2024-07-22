namespace WebApplication14.Models
{
    public class Submission
    {
        public int SubmissionId { get; set; }
        public int AssignmentsId { get; set; }
        public int StudentId { get; set; }
        public DateTime SubmissionDate { get; set; }
        public byte[]? Content { get; set; }

        public string Feedback { get; set; } = string.Empty;
        public Assignments? assignments { get; set; }
        public Student? student { get; set; }

        public Grades? Grades { get; set; }
    }
}
