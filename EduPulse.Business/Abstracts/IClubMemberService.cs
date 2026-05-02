using EduPulse.DTOs.ClubMembers;
using EduPulse.DTOs.Common;

namespace EduPulse.Business.Abstracts;

public interface IClubMemberService
{
    Task<Result<List<ClubMemberListDto>>> GetAllForCurrentUserAsync(string? roleName, string? schoolId);
    Task<Result<List<ClubMemberListDto>>> GetByClubIdForCurrentUserAsync(string clubId, string? roleName, string? schoolId);
    Task<Result<List<ClubMemberListDto>>> GetByStudentIdForCurrentUserAsync(string studentId, string? roleName, string? schoolId);

    Task<Result> CreateAsync(CreateClubMemberDto dto, string? roleName, string? schoolId);
    Task<Result> DeleteAsync(string id, string? roleName, string? schoolId);
}