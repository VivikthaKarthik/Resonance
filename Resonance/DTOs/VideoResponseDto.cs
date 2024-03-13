namespace ResoClassAPI.DTOs
{
    public class VideoResponseDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ThumbNail { get; set; }
        public string SourceUrl { get; set; }
        public string SubTopic { get; set; }
        public string Topic { get; set; }
        public string Chapter { get; set; }
        public bool HomeDisplay { get; set; }
    }
}
