namespace ResoClassAPI.DTOs
{
    public class TimeSpentAnalysisDto
    {
        public string Title { get; set; }
        public string Course { get; set; }
        public string Class { get; set; }

        public int AverageTimeSpentOnEachQuestion { get; set; }
        public int AverageTimeSpentOnEasyQuestions { get; set; }
        public int AverageTimeSpentOnMediumQuestions { get; set; }
        public int AverageTimeSpentOnDifficultQuestions { get; set; }

        public List<ItemWiseTimeAnalysisReportDto> Reports { get; set; }
    }
    public class SubjectWiseTimeAnalysisDto : TimeSpentAnalysisDto
    {
        public string Subject { get; set; }
    }
    public class ChapterWiseTimeAnalysisDto : SubjectWiseTimeAnalysisDto
    {
        public string Chapter { get; set; }
    }
    public class TopicWiseTimeAnalysisDto : ChapterWiseTimeAnalysisDto
    {
        public string Topic { get; set; }
    }
    public class ItemWiseTimeAnalysisReportDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<AssessmentLevelTimeAnalysisReportDto> LevelReports { get; set; }
    }

    public class AssessmentLevelTimeAnalysisReportDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int AverageTime { get; set; }
    }
}
