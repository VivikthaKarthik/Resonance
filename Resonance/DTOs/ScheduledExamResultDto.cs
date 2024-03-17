namespace ResoClassAPI.DTOs
{
    public class ScheduledExamResultDto
    {
        public string Course { get; set; }
        public string Subject { get; set; }
        public DateTime StartDate { get; set; }
        public string ExamName { get; set; }
        public string StudentName { get; set; }
        public int TotalQuestions { get; set; }
        public int QuestionsAttempted { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public int TotalScore { get; set; }
    }
}
