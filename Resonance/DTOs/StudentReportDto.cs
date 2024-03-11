namespace ResoClassAPI.DTOs
{
    public class StudentReportDto
    {
        public string StudentName { get; set; }
        public string CourseName { get; set; }
        public List<ItemWiseWeightage> SubjectsWeightage { get; set; }
        public List<ListItemDto> PracticedFromSubjects { get; set; }
        public List<ListItemDto> PracticedFromChapters { get; set; }
        public List<ListItemDto> PracticedFromTopics { get; set; }
        public int TotalAttempted { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public SubjectAnalysisDto CorrectAnswersAnalysis { get; set; }
        public SubjectAnalysisDto WrongAnswersAnalysis { get; set; }
    }
}
