using EduPulse.Entities.Common;

namespace EduPulse.Entities.Teachers
{
    public class Teacher : BaseEntity
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        public string Branch { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;
        public string? Email { get; set; }

        public string SchoolId { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }
}
