namespace ResoClassAPI.DTOs
{
    public class SubjectReportDto
    {
        public long SubjectId { get; set; }
        public List<ListItemDto> PracticedFromChapters { get; set; }
        public List<ListItemDto> PracticedFromTopics { get; set; }
        public int TotalAttempted { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public ChapterAnalysisDto CorrectAnswersAnalysis { get; set; }
        public ChapterAnalysisDto WrongAnswersAnalysis { get; set; }
    }
}
