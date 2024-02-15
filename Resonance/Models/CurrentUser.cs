namespace ResoClassAPI.Models
{
    public class CurrentUser
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string DeviceId { get; set; }
    }
}
