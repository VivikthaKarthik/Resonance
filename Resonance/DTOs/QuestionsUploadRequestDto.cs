namespace ResoClassAPI.DTOs
{
    public class QuestionsUploadRequestDto
    {
        public long CourseId { get; set; }
        public long SubjectId { get; set; }
        public long? ChapterId { get; set; }
        public long? TopicId { get; set; }
        public long? SubTopicId { get; set; }
    }
}
