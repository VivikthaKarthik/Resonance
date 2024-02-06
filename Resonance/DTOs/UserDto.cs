﻿namespace ResoClassAPI.DTOs
{
    public class UserDto
    {
        public long? Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string Email { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string Role { get; set; }
    }
}
