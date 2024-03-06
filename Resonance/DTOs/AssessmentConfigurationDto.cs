namespace ResoClassAPI.DTOs
{
    public class AssessmentConfigurationDto
    {
        public long Id { get; set; }

        public long CourseId { get; set; }

        public int MaximumQuestions { get; set; }

        public int MarksPerQuestion { get; set; }

        public bool HasNegativeMarking { get; set; }

        public int? NegativeMarksPerQuestion { get; set; }

        public int PassMarkks { get; set; }

        public int? TimeDuration { get; set; }
    }
}
