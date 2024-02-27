﻿namespace ResoClassAPI.Utilities
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
        CorrectAnswer
    }
}
