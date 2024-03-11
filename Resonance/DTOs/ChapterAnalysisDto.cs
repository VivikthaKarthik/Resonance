namespace ResoClassAPI.DTOs
{
    public class ChapterAnalysisDto : TopicAnalysisDto
    {
        public List<ItemWiseAnalysisDto> ChapterWiseAnalysis { get; set; }
    }
}
