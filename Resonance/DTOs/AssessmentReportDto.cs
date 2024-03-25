namespace ResoClassAPI.DTOs
{
    public class AssessmentReportDto
    {
        public long AssessmentId { get; set; }
        public string AssessmentName { get; set; }
        public string AssessmentType { get; set; }
        public string AssessmentLevel { get; set; }
        public DateTime PracticedOn { get; set; }
        public int TotalAttempted { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public string ChapterName { get; set; }
        public string TopicName { get; set; }
        public string SubTopicName { get; set; }
        public List<QuestionKeyDto> Keys { get; set; }

    }

}
