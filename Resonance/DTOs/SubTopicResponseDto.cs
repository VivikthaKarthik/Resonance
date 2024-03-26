namespace ResoClassAPI.DTOs
{
    public class SubTopicResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public string SourceUrl { get; set; }
        public string Description { get; set; }
        public bool HomeDisplay { get; set; }
        public string ClassNotesUrl { get; set; }
        public string ExtractUrl { get; set; }
        public long TopicId { get; set; } 
        public string TopicName { get; set; }
        public long ChapterId { get; set; }
        public string ChapterName { get; set; }
        public long SubjectId { get; set; }
        public string SubjectName { get; set; }

    }
}
