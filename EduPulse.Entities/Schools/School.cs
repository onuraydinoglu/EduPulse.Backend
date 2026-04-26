using EduPulse.Entities.Common;

namespace EduPulse.Entities.Schools
{
    public class School : BaseEntity
    {
        public string Name { get; set; } = null!;

        public string City { get; set; } = null!;
        public string District { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;
        public string? Email { get; set; }

        public string? PrincipalName { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
