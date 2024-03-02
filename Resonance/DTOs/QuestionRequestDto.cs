namespace ResoClassAPI.DTOs
{
    public class QuestionRequestDto
    {
        public List<long> ChapterIds { get; set; }
        public List<long> TopicIds { get; set; }
        public List<long> SubTopicIds { get; set; }
    }
}
