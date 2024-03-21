namespace ResoClassAPI.DTOs
{
    public class ChaptersViewDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public bool IsRecommended { get; set; }
        public string Subject { get; set; }
        public string Class { get; set; }
        public string Course { get; set; }
    }
}
