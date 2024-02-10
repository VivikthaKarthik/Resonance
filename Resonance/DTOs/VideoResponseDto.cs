namespace ResoClassAPI.DTOs
{
    public class VideoResponseDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ThumbNail { get; set; } = null!;
        public string SourceUrl { get; set; } = null!;
    }
}
