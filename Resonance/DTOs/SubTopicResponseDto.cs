﻿namespace ResoClassAPI.DTOs
{
    public class SubTopicResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Thumbnail { get; set; } = null!;
        public long TopicId { get; set; } 
        public string TopicName { get; set; }
        public long ChapterId { get; set; }
        public string ChapterName { get; set; }
        public long SubjectId { get; set; }
        public string SubjectName { get; set; }

    }
}