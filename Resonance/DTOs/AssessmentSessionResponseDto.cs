namespace ResoClassAPI.DTOs
{
    public class AssessmentSessionResponseDto
    {
        public long Id { get; set; }
        public string PracticeSession { get; set; }
        public DateTime PracticedOn { get; set; }
        public int AttemptedQuestions { get; set; }
        public int Score { get; set; }
    }
}
