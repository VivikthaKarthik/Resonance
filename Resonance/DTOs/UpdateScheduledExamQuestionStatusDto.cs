namespace ResoClassAPI.DTOs
{
    public class UpdateScheduledExamQuestionStatusDto
    {
        public long AssessmentId { get; set; }
        public long QuestionId { get; set; }
        public string SelectedAnswer { get; set; }
        public bool Result { get; set; }
    }
}
