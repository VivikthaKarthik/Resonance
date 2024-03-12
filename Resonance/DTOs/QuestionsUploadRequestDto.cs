namespace ResoClassAPI.DTOs
{
    public class QuestionsUploadRequestDto
    {
        public string Course { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Chapter { get; set; }
        public string Topic { get; set; }
        public string SubTopic { get; set; }
    }
}
