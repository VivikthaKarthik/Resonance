namespace ResoClassAPI.DTOs
{
    public class ChapterResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Thumbnail { get; set; } = null!;
        public bool IsRecommended { get; set; }
        public long SubjectId { get; set; }
        public string SubjectName { get; set; }
    }
}
