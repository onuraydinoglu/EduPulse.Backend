using EduPulse.DTOs.ClubMembers;
using EduPulse.DTOs.Clubs;
using EduPulse.DTOs.Common;

namespace EduPulse.Business.Abstracts;

public interface IClubService
{
    Task<Result<List<ClubListDto>>> GetAllForCurrentUserAsync(string? roleName, string? schoolId);
    Task<Result<ClubListDto>> GetByIdForCurrentUserAsync(string id, string? roleName, string? schoolId);

    Task<Result> CreateAsync(CreateClubDto dto, string? roleName, string? schoolId);
    Task<Result> UpdateAsync(UpdateClubDto dto, string? roleName, string? schoolId);
    Task<Result> DeleteAsync(string id, string? roleName, string? schoolId);
}