namespace ResoClassAPI.DTOs
{
    public class TopicResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; }
        public string Thumbnail { get; set; } = null!;
        public long ChapterId { get; set; }
        public string Chapter { get; set; }
        public long SubjectId { get; set; }
        public string Subject { get; set; }
        public long ClassId { get; set; }
        public string Class { get; set; }
        public long CourseId { get; set; }
        public string Course { get; set; }
    }
}
