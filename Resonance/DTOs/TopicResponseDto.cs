namespace ResoClassAPI.DTOs
{
    public class TopicResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; }
        public string Thumbnail { get; set; } = null!;
        public long ChapterId { get; set; }
        public string ChapterName { get; set; }
        public long SubjectId { get; set; }
        public string SubjectName { get; set; }
    }
}
