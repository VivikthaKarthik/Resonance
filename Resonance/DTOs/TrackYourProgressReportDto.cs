namespace ResoClassAPI.DTOs
{
    public class TrackYourProgressReportDto
    {
        public string Title { get; set; }
        public string Course { get; set; }
        public string Class { get; set; }

        public int TotalQuestions { get; set; }
        public int TotalQuestionsAttempted { get; set; }
        public int TotalCorrect { get; set; }
        public int TotalWrong { get; set; }

        public double CorrectAnswersPercentage { get; set; }
        public double WrongAnswersPercentage { get; set; }
        public List<ItemWiseReportDto> Reports { get; set; }
    }
    public class SubjectWiseTestDto : TrackYourProgressReportDto
    {
        public string Subject { get; set; }
    }
    public class ChapterWiseTestDto : SubjectWiseTestDto
    {
        public string Chapter { get; set; }
    }
    public class TopicWiseTestDto : ChapterWiseTestDto
    {
        public string Topic { get; set; }
    }

    public class ItemWiseReportDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<AssessmentLevelReportDto> LevelReports { get; set; }
    }

    public class AssessmentLevelReportDto
    {
        public long Id { get; set; }
        public long AssessmentId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalQuestionsAttempted { get; set; }
        public int TotalCorrect { get; set; }
        public int TotalWrong { get; set; }
    }
}
