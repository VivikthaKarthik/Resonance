namespace ResoClassAPI.DTOs
{
    public class SearchResponseDto
    {
        public List<ListItemDto> Chapters { get; set; }
        public List<ListItemDto> Topics { get; set; }
        public List<ListItemDto> SubTopics { get; set; }
    }
}
