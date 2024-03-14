namespace ResoClassAPI.Utilities
{
    public static class SqlTableName
    {
        public const string User = "User";
        public const string Chapter = "Chapter";
        public const string Subject = "Subject";
        public const string Topic = "Topic";
        public const string SubTopic = "SubTopic";
        public const string Video = "Video";
        public const string Course = "Course";
        public const string Student = "Student";
    }
    public enum CurrentElement
    {
        Question,
        FirstAnswer,
        SecondAnswer,
        ThirdAnswer,
        FourthAnswer,
        CorrectAnswer,
        DifficultyLevel
    }

    public enum clientType
    {
        WEB,
        Mobile
    }

    public static class  QuestionAndAnswerTags
    {
        public const string QuestionImageOpeningTag = "<QuestionImageOpeningTag>";
        public const string AnswerImageOpeningTag = "<AnswerImageOpeningTag>";
        public const string ImageClosingTag = "<ImageClosingTag>";
        public const string QuestionTextOpeningTag = "<QuestionTextOpeningTag>";
        public const string AnswerTextOpeningTag = "<AnswerTextOpeningTag>";
        public const string TextClosingTag = "<TextClosingTag>";
        public const string NewLineTag = "<NewLineTag>";
    }
}
