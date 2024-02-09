namespace ResoClassAPI.DTOs
{
    public class SubTopicDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public int TopicId { get; set; }
        public string Thumbnail { get; set; } = null!;
        
    }
}
