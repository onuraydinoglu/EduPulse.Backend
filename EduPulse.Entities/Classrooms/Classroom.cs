using EduPulse.Entities.Common;

namespace EduPulse.Entities.Classrooms
{
    public class Classroom : BaseEntity
    {        
        public string SchoolId { get; set; } = null!;

        public int Grade { get; set; }      // 9, 10, 11, 12
        public string Section { get; set; } = null!; // A, B, C

        public string? TeacherId { get; set; } // Sınıf öğretmeni

        public bool IsActive { get; set; } = true;
    }
}
