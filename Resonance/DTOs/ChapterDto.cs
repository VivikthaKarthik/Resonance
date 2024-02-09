namespace ResoClassAPI.DTOs
{
    public class ChapterDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public int SubjectId { get; set; }
        public int CourseId { get; set; }
        public string Thumbnail { get; set; } = null!;
        public bool IsRecommended { get; set; }
       
    }
}
