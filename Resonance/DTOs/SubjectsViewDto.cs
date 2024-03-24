namespace ResoClassAPI.DTOs
{
    public class SubjectsViewDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long CourseId { get; set; }
        public string Course { get; set; }
        public long ClassId { get; set; }
        public string Class { get; set; }
        public string ColorCode { get; set; }
    }
}
