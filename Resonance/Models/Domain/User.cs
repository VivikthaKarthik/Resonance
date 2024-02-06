using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class User
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public int RoleId { get; set; }

    public string Password { get; set; } = null!;

    public string? DeviceId { get; set; }

    public string? RegistrationId { get; set; }

    public string? Longitude { get; set; }

    public string? Latitude { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? LastLoginDate { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public virtual Role Role { get; set; } = null!;
}
