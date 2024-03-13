namespace ResoClassAPI.DTOs
{
    public class VideoDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ThumbNail { get; set; } = null!;
        public string SourceUrl { get; set; } = null!;
        public int SubTopicId { get; set; }
        public int TopicId { get; set; }
        public int ChapterId { get; set; }
        public bool HomeDisplay { get; set; }
        
    }
}
