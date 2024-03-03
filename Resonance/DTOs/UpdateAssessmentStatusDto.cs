namespace ResoClassAPI.DTOs
{
    public class UpdateAssessmentStatusDto
    {
        public long AssessmentId { get; set; }
        public long QuestionId { get; set; }
        public bool Result { get; set; }
    }
}
