using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Classrooms;
using EduPulse.Entities.Classrooms;
using EduPulse.Repository.Abstracts;

namespace EduPulse.Business.Concretes;

public class ClassroomService : IClassroomService
{
    private readonly IClassroomRepository _classroomRepository;
    private readonly ISchoolRepository _schoolRepository;

    public ClassroomService(
        IClassroomRepository classroomRepository,
        ISchoolRepository schoolRepository)
    {
        _classroomRepository = classroomRepository;
        _schoolRepository = schoolRepository;
    }

    public async Task<List<ClassroomListDto>> GetAllAsync()
    {
        var classrooms = await _classroomRepository.GetAllAsync();

        return classrooms.Select(x => new ClassroomListDto
        {
            Id = x.Id,
            SchoolId = x.SchoolId,
            Grade = x.Grade,
            Section = x.Section,
            TeacherId = x.TeacherId,
            IsActive = x.IsActive
        }).ToList();
    }

    public async Task<List<ClassroomListDto>> GetBySchoolIdAsync(string schoolId)
    {
        var classrooms = await _classroomRepository.GetBySchoolIdAsync(schoolId);

        return classrooms.Select(x => new ClassroomListDto
        {
            Id = x.Id,
            SchoolId = x.SchoolId,
            Grade = x.Grade,
            Section = x.Section,
            TeacherId = x.TeacherId,
            IsActive = x.IsActive
        }).ToList();
    }

    public async Task<ClassroomListDto?> GetByIdAsync(string id)
    {
        var classroom = await _classroomRepository.GetByIdAsync(id);

        if (classroom is null)
            return null;

        return new ClassroomListDto
        {
            Id = classroom.Id,
            SchoolId = classroom.SchoolId,
            Grade = classroom.Grade,
            Section = classroom.Section,
            TeacherId = classroom.TeacherId,
            IsActive = classroom.IsActive
        };
    }

    public async Task CreateAsync(CreateClassroomDto dto)
    {
        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);

        if (school is null)
            throw new Exception("Okul bulunamadı.");

        var classroom = new Classroom
        {
            SchoolId = dto.SchoolId,
            Grade = dto.Grade,
            Section = dto.Section,
            TeacherId = dto.TeacherId,
            IsActive = true
        };

        await _classroomRepository.CreateAsync(classroom);
    }

    public async Task UpdateAsync(UpdateClassroomDto dto)
    {
        var classroom = await _classroomRepository.GetByIdAsync(dto.Id);

        if (classroom is null)
            throw new Exception("Sınıf bulunamadı.");

        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);

        if (school is null)
            throw new Exception("Okul bulunamadı.");

        classroom.SchoolId = dto.SchoolId;
        classroom.Grade = dto.Grade;
        classroom.Section = dto.Section;
        classroom.TeacherId = dto.TeacherId;
        classroom.IsActive = dto.IsActive;

        await _classroomRepository.UpdateAsync(classroom);
    }

    public async Task DeleteAsync(string id)
    {
        var classroom = await _classroomRepository.GetByIdAsync(id);

        if (classroom is null)
            throw new Exception("Sınıf bulunamadı.");

        await _classroomRepository.DeleteAsync(id);
    }
}