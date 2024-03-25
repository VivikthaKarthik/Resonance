namespace ResoClassAPI.DTOs
{
    public class QuestionResponseDto
    {
        public long AssessmentId { get; set; }
        public string AssessmentName { get; set; }
        public int MarksPerQuestion { get; set; }
        public int TotalQuestions { get; set; }
        public bool HasNegativeMarking { get; set; }
        public int NegativeMarksPerQuestion { get; set; }
        public int MaximumTimeToComplete { get; set; }
        public List<QuestionData> Questions { get; set; }

    }

    public class QuestionData
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string FirstAnswer { get; set; }
        public string SecondAnswer { get; set; }
        public string ThirdAnswer { get; set; }
        public string FourthAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public string DifficultyLevel { get; set; }
        public long? SubTopicId { get; set; }
        public long? TopicId { get; set; }
        public long? ChapterId { get; set; }
    }
}
