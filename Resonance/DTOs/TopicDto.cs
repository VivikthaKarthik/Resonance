namespace ResoClassAPI.DTOs
{
    public class TopicDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; }
        public int ChapterId { get; set; } 
        public string Thumbnail { get; set; } = null!;
        
    }
}
