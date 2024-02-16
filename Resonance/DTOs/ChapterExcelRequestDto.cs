﻿namespace ResoClassAPI.DTOs
{
    public class ChapterExcelRequestDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Subject { get; set; }
        public string Course { get; set; }
        public string Thumbnail { get; set; } = null!;
        public bool? IsRecommended { get; set; }
    }
}
