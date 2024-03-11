namespace ResoClassAPI.DTOs
{
    public class ChapterReportDto
    {
        public long ChapterId { get; set; }
        public List<ListItemDto> PracticedFromTopics { get; set; }
        public int TotalAttempted { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public TopicAnalysisDto CorrectAnswersAnalysis { get; set; }
        public TopicAnalysisDto WrongAnswersAnalysis { get; set; }
    }
}
