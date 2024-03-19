namespace ResoClassAPI.DTOs
{
    public class StudentProfileDto
    {
        public string AdmissionId { get; set; }
        public DateTime AdmissionDate { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Landmark { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PinCode { get; set; }
        public string Gender { get; set; }
        public long CourseId { get; set; }
        public string CourseName { get; set; }
        public string ClassName {  get; set; }
        public string MobileNumber { get; set; }
        public string AlternateMobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
