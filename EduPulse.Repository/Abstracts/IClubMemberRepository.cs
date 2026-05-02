using EduPulse.Entities.ClubMembers;

namespace EduPulse.Repository.Abstracts;

public interface IClubMemberRepository
{
    Task<List<ClubMember>> GetByClubIdAsync(string clubId);
    Task<List<ClubMember>> GetByStudentIdAsync(string studentId);

    Task<ClubMember?> GetActiveByClubIdAndStudentIdAsync(string clubId, string studentId);
    Task<ClubMember?> GetAnyByClubIdAndStudentIdAsync(string clubId, string studentId);

    Task<int> GetActiveMemberCountByClubIdAsync(string clubId);

    Task CreateAsync(ClubMember clubMember);
    Task ReactivateAsync(string clubId, string studentId);
    Task DeleteAsync(string clubId, string studentId);
}