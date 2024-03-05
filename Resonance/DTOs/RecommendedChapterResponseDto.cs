namespace ResoClassAPI.DTOs
{
    public class RecommendedChapterResponseDto
    {
        public RecommendedChapterResponseDto()
        {
            RecommendedChapters = new List<RecommendedChapterDto>();
        }
        public long SubjectId { get; set; }
        public string SubjectName { get; set; }

        public List<RecommendedChapterDto> RecommendedChapters { get; set; }
    }

    public class RecommendedChapterDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; }
        public string Thumbnail { get; set; } = null!; 

    }
}
