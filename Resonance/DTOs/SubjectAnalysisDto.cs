namespace ResoClassAPI.DTOs
{
    public class SubjectAnalysisDto : ChapterAnalysisDto
    {
        public List<ItemWiseAnalysisDto> SubjectWiseAnalysis { get; set; }
    }
}
