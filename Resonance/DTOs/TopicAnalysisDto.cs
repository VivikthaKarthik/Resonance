namespace ResoClassAPI.DTOs
{
    public class TopicAnalysisDto
    {
        public List<ItemWiseAnalysisDto> DifficultyLevelAnalysis { get; set; }
        public List<ItemWiseAnalysisDto> TopicWiseAnalysis { get; set; }
    }
}
