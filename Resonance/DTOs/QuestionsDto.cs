namespace ResoClassAPI.DTOs
{
    public class QuestionsDto
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string FirstAnswer { get; set; }
        public string SecondAnswer { get; set; }
        public string ThirdAnswer { get; set; }
        public string FourthAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public string DifficultyLevel { get; set; }
        public string IsPreviousYearQuestion { get; set; }
    }
}
