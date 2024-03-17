namespace ResoClassAPI.DTOs
{
    public class ScheduledExamQuestionsResponseDto
    {
        public long ScheduleExamSessionId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MarksPerQuestion { get; set; }
        public int TotalQuestions { get; set; }
        public bool HasNegativeMarking { get; set; }
        public int NegativeMarksPerQuestion { get; set; }
        public int MaxAllowedTime { get; set; }
        public long CourseId { get; set; }
        public string Course { get; set; }
        public long SubjectId { get; set; }
        public string Subject { get; set; }
        public List<ScheduledExamQuestionData> Questions { get; set; }

    }

    public class ScheduledExamQuestionData
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string FirstAnswer { get; set; }
        public string SecondAnswer { get; set; }
        public string ThirdAnswer { get; set; }
        public string FourthAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public string DifficultyLevel { get; set; }
    }
}
