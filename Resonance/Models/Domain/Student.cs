using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class Student
{
    public long Id { get; set; }

    public string AdmissionId { get; set; } = null!;

    public DateTime AdmissionDate { get; set; }

    public string Name { get; set; } = null!;

    public string FatherName { get; set; } = null!;

    public string MotherName { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public string AddressLine1 { get; set; } = null!;

    public string? AddressLine2 { get; set; }

    public string? Landmark { get; set; }

    public long CityId { get; set; }

    public long StateId { get; set; }

    public string PinCode { get; set; } = null!;

    public long BranchId { get; set; }

    public string Gender { get; set; } = null!;

    public long CourseId { get; set; }

    public long ClassId { get; set; }

    public string MobileNumber { get; set; } = null!;

    public string? AlternateMobileNumber { get; set; }

    public string EmailAddress { get; set; } = null!;

    public string? DeviceId { get; set; }

    public string? FirebaseId { get; set; }

    public string Password { get; set; } = null!;

    public string? Longitude { get; set; }

    public string? Latitude { get; set; }

    public DateTime? LastLoginDate { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public bool? IsPasswordChangeRequired { get; set; }

    public string? ProfilePicture { get; set; }

    public virtual ICollection<AssessmentSession> AssessmentSessions { get; } = new List<AssessmentSession>();

    public virtual Branch Branch { get; set; } = null!;

    public virtual City City { get; set; } = null!;

    public virtual Class Class { get; set; } = null!;

    public virtual ICollection<ScheduledExamSession> ScheduledExamSessions { get; } = new List<ScheduledExamSession>();

    public virtual State State { get; set; } = null!;
}
