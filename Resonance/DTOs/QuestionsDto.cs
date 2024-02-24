namespace ResoClassAPI.DTOs
{
    public class QuestionsDto
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public AnswerDto FirstAnswer { get; set; }
        public AnswerDto SecondAnswer { get; set; }
        public AnswerDto ThirdAnswer { get; set; }
        public AnswerDto FourthAnswer { get; set; }
        public string CorrectAnswer { get; set; }
    }

    public class AnswerDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public byte[] Image { get; set; } // Byte array to store image

        public AnswerDto(string answerText, byte[] image)
        {
            if (!string.IsNullOrEmpty(answerText))
            {
                Text = answerText;
            }

            if (image != null && image.Length > 0)
            {
                Image = image;
            }
        }
    }
    public enum CurrentElement
    {
        Question,
        FirstAnswer,
        SecondAnswer,
        ThirdAnswer,
        FourthAnswer,
        CorrectAnswer
    }
}
