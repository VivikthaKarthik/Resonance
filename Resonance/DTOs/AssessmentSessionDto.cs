namespace ResoClassAPI.DTOs
{
    public class AssessmentSessionDto
    {
        public long Id { get; set; }

        public string AssessmentType { get; set; } = null!;

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public bool? Result { get; set; }
    }
}
