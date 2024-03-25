namespace ResoClassAPI.DTOs
{
    public class DifficultyLevelAnalysisDto
    {
        public string Title { get; set; }
        public string Course { get; set; }
        public string Class { get; set; }

        public List<AssessmentLevelDifficultyLevelAnalysisDto> TotalQuestionAnalysis { get; set; }
        public List<ItemWiseDifficultyLevelAnalysisDto> Reports { get; set; }
    }

    public class SubjectWiseDifficultyLevelAnalysisDto : DifficultyLevelAnalysisDto
    {
        public string Subject { get; set; }
    }
    public class ChapterWiseDifficultyLevelAnalysisDto : SubjectWiseDifficultyLevelAnalysisDto
    {
        public string Chapter { get; set; }
    }
    public class TopicWiseDifficultyLevelAnalysisDto : ChapterWiseDifficultyLevelAnalysisDto
    {
        public string Topic { get; set; }
    }
    public class ItemWiseDifficultyLevelAnalysisDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<AssessmentLevelDifficultyLevelAnalysisDto> LevelReports { get; set; }
    }

    public class AssessmentLevelDifficultyLevelAnalysisDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalQuestionsAttempted { get; set; }
        public int TotalCorrect { get; set; }
        public int TotalWrong { get; set; }
    }

}
