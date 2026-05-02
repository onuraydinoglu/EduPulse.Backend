using EduPulse.Entities.ClubMembers;

namespace EduPulse.Repository.Abstracts;

public interface IClubMemberRepository
{
    Task<List<ClubMember>> GetAllAsync();
    Task<List<ClubMember>> GetBySchoolIdAsync(string schoolId);
    Task<List<ClubMember>> GetByClubIdAsync(string clubId);
    Task<List<ClubMember>> GetByStudentIdAsync(string studentId);

    Task<ClubMember?> GetByIdAsync(string id);
    Task<ClubMember?> GetByClubIdAndStudentIdAsync(string clubId, string studentId);

    Task<int> GetActiveMemberCountByClubIdAsync(string clubId);

    Task CreateAsync(ClubMember clubMember);
    Task DeleteAsync(string id);
}