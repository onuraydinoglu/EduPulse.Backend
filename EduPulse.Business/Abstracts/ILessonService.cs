using EduPulse.DTOs.Common;
using EduPulse.DTOs.Lessons;

namespace EduPulse.Business.Abstracts;

public interface ILessonService
{
    Task<Result<List<LessonListDto>>> GetAllAsync();
    Task<Result<List<LessonListDto>>> GetBySchoolIdAsync(string schoolId);
    Task<Result<LessonListDto>> GetByIdAsync(string id);
    Task<Result> CreateAsync(CreateLessonDto dto);
    Task<Result> UpdateAsync(UpdateLessonDto dto);
    Task<Result> DeleteAsync(string id);
}