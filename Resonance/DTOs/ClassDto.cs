namespace ResoClassAPI.DTOs
{
    public class ClassDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Thumbnail { get; set; }
        public long CourseId { get; set; }
        public string? Course { get; set; }
    }
}
