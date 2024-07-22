namespace WebApplication14.Models
{
    public class Grades
    {
        public int GradesId { get; set; }
        public int SubmissionId { get; set; }
        public double Grade { get; set; }

        public string GradedBy { get; set; } = string.Empty;

        public Submission? submission { get; set; }
    }
}
