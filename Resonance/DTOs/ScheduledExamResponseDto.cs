namespace ResoClassAPI.DTOs
{
    public class ScheduledExamResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MarksPerQuestion { get; set; }
        public int NegativeMarksPerQuestion { get; set; }
        public int MaxAllowedTime { get; set; }
        public string Course { get; set; }
        public string Subject { get; set; }
    }
}
