namespace ResoClassAPI.DTOs
{
    public class StudentLoginResponseDto
    {
        public bool IsPasswordChangeRequired { get; set; }
        public string Token { get;set; }
    }
}
