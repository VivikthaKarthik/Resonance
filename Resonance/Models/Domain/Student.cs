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

    public string AddressLine2 { get; set; } = null!;

    public string Landmark { get; set; } = null!;

    public long CityId { get; set; }

    public long StateId { get; set; }

    public string PinCode { get; set; } = null!;

    public string BranchId { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public long CourseId { get; set; }

    public string MobileNumber { get; set; } = null!;

    public string AlternateMobileNumber { get; set; } = null!;

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

    public virtual ICollection<ExamResult> ExamResults { get; } = new List<ExamResult>();
}
