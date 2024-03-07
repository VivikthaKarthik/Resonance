namespace ResoClassAPI.DTOs
{
    public class AssessmentReportDto
    {
        public long AssessmentId { get; set; }
        public DateTime PracticedOn { get; set; }
        public List<ListItemDto> PracticedFromChapters { get; set; }
        public List<ListItemDto> PracticedFromTopics { get; set; }
        public int TotalAttempted { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public AssessmentAnalysisDto CorrectAnswersAnalysis { get; set; }
        public AssessmentAnalysisDto WrongAnswersAnalysis { get; set; }

    }

    public class AssessmentAnalysisDto
    {
        public List<ItemWiseAnalysisDto> DifficultyLevelAnalysis { get; set; }
        public List<ItemWiseAnalysisDto> ChapterWiseAnalysis { get; set; }
        public List<ItemWiseAnalysisDto> TopicWiseAnalysis { get; set; }
    }
    public class ItemWiseAnalysisDto
    {
        public string Name { get; set; }
        public int QuestionsCount { get; set; }
    }
}
