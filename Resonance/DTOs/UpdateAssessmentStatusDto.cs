namespace ResoClassAPI.DTOs
{
    public class UpdateAssessmentStatusDto
    {
        public long AssessmentId { get; set; }
        public long QuestionId { get; set; }
        public long? ChapterId { get; set; }
        public long? TopicId { get; set; }
        public long? SubTopicId { get; set; }
        public string DifficultyLevel { get; set; }
        public string SelectedAnswer { get; set; }
        public bool Result { get; set; }
    }
}
