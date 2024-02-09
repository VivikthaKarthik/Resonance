namespace ResoClassAPI.Models
{
    public class CurrentChapter
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int SubjectId { get; set; }
        public int CourseId { get; set; }
        public string Thumbnail { get; set; } = null!;
        public bool IsRecommended { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; } = null!;
        public DateTime ModifiedOn { get; set; }
    }
}
