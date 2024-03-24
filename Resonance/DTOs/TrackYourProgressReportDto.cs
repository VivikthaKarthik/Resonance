namespace ResoClassAPI.DTOs
{
    public class TrackYourProgressReportDto
    {
        public ChapterWiseTestDto ChapterWiseTest { get; set; }
        public TopicWiseTestDto TopicWiseTest { get; set; }
        public QuickPracticeTestDto QuickPracticeTest { get; set; }
    }

    public class ChapterWiseTestDto
    {
        public string Title { get; set; }
        public string Course { get; set; }
        public string Class { get; set; }
        public string Subject { get; set; }

        public int TotalQuestions { get; set; }
        public int TotalQuestionsAttempted { get; set; }
        public int TotalCorrect { get; set; }
        public int TotalWrong { get; set; }

        public decimal CorrectAnswersPercentage { get; set; }
        public decimal WrongAnswersPercentage { get; set; }
        public List<ItemWiseReportDto> ChapterReports { get; set; }
    }
    public class TopicWiseTestDto
    {
        public string Title { get; set; }
        public string Course { get; set; }
        public string Class { get; set; }
        public string Subject { get; set; }
        public string Chapter { get; set; }

        public int TotalQuestions { get; set; }
        public int TotalQuestionsAttempted { get; set; }
        public int TotalCorrect { get; set; }
        public int TotalWrong { get; set; }

        public decimal CorrectAnswersPercentage { get; set; }
        public decimal WrongAnswersPercentage { get; set; }
        public List<ItemWiseReportDto> ChapterReports { get; set; }
    }
    public class QuickPracticeTestDto
    {
        public string Title { get; set; }
        public string Course { get; set; }
        public string Class { get; set; }
        public string Subject { get; set; }
        public string Chapter { get; set; }
        public string Topic { get; set; }

        public int TotalQuestions { get; set; }
        public int TotalQuestionsAttempted { get; set; }
        public int TotalCorrect { get; set; }
        public int TotalWrong { get; set; }

        public decimal CorrectAnswersPercentage { get; set; }
        public decimal WrongAnswersPercentage { get; set; }
        public List<ItemWiseReportDto> ChapterReports { get; set; }
    }

    public class ItemWiseReportDto
    {
        public string Name { get; set; }
        public List<AssessmnetLevelReportDto> LevelReports { get; set; }
    }

    public class AssessmnetLevelReportDto
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalCorrect { get; set; }
        public int TotalWrong { get; set; }
    }
}
