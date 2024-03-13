namespace ResoClassAPI.DTOs
{
    public class ChapterRequestDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; }
        public long SubjectId { get; set; } = 0;
        public string? Thumbnail { get; set; }
        public bool? IsRecommended { get; set; }
       
    }
}
