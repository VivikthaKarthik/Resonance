namespace ResoClassAPI.DTOs
{
    public class SearchResponseDto
    {
        public List<SearchItem> Chapters { get; set; }
        public List<SearchItem> Topics { get; set; }
        public List<SearchItem> SubTopics { get; set; }
    }

    public class SearchItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
