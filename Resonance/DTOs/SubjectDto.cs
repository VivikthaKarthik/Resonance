namespace ResoClassAPI.DTOs
{
    public class SubjectDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Thumbnail { get; set; }
        public long CourseId { get; set; }
        public string? CourseName { get; set; }
        public string? ColorCode { get; set; }
    }
}
