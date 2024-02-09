namespace ResoClassAPI.DTOs
{
    public class StudentLoginResponseDto
    {
        public long StudentId { get; set; }
        public string Name { get; set; }
        public long CourseId { get; set; }
        public string CourseName { get; set; }
        public string Token { get;set; }
    }
}
