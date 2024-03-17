namespace ResoClassAPI.DTOs
{
    public class ScheduledExamRequestDto
    {
        public string Name { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MarksPerQuestion { get; set; }
        public int NegativeMarksPerQuestion { get; set; }
        public int MaxAllowedTime { get; set; }
        public long CourseId { get; set; }
        public long SubjectId { get; set; }
    }
}
