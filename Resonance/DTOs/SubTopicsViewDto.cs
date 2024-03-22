namespace ResoClassAPI.DTOs
{
    public class SubTopicsViewDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public string SourceUrl { get; set; }
        public bool HomeDisplay { get; set; }
        public long TopicId { get; set; }
        public string Topic { get; set; }
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
