using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Classrooms;
using EduPulse.Entities.Classrooms;
using EduPulse.Repository.Abstracts;

namespace EduPulse.Business.Concretes;

public class ClassroomService : IClassroomService
{
    private readonly IClassroomRepository _classroomRepository;
    private readonly ISchoolRepository _schoolRepository;
    private readonly ITeacherRepository _teacherRepository;

    public ClassroomService(
        IClassroomRepository classroomRepository,
        ISchoolRepository schoolRepository,
        ITeacherRepository teacherRepository)
    {
        _classroomRepository = classroomRepository;
        _schoolRepository = schoolRepository;
        _teacherRepository = teacherRepository;
    }

    public async Task<List<ClassroomListDto>> GetAllAsync()
    {
        var classrooms = await _classroomRepository.GetAllAsync();

        var teacherIds = classrooms
            .Where(x => x.TeacherId != null)
            .Select(x => x.TeacherId!)
            .Distinct()
            .ToList();

        var teachers = await _teacherRepository.GetAllAsync();

        return classrooms.Select(x => new ClassroomListDto
        {
            Id = x.Id,
            SchoolId = x.SchoolId,
            Grade = x.Grade,
            Section = x.Section,
            TeacherId = x.TeacherId,
            TeacherName = teachers
                .FirstOrDefault(t => t.Id == x.TeacherId)?
                .FirstName + " " +
                teachers.FirstOrDefault(t => t.Id == x.TeacherId)?.LastName,
            IsActive = x.IsActive
        }).ToList();
    }

    public async Task<List<ClassroomListDto>> GetBySchoolIdAsync(string schoolId)
    {
        var classrooms = await _classroomRepository.GetBySchoolIdAsync(schoolId);
        var teachers = await _teacherRepository.GetAllAsync();

        return classrooms.Select(x => new ClassroomListDto
        {
            Id = x.Id,
            SchoolId = x.SchoolId,
            Grade = x.Grade,
            Section = x.Section,
            TeacherId = x.TeacherId,
            TeacherName = teachers
                .FirstOrDefault(t => t.Id == x.TeacherId)?
                .FirstName + " " +
                teachers.FirstOrDefault(t => t.Id == x.TeacherId)?.LastName,
            IsActive = x.IsActive
        }).ToList();
    }

    public async Task<ClassroomListDto?> GetByIdAsync(string id)
    {
        var classroom = await _classroomRepository.GetByIdAsync(id);

        if (classroom is null)
            return null;

        var teacher = classroom.TeacherId != null
            ? await _teacherRepository.GetByIdAsync(classroom.TeacherId)
            : null;

        return new ClassroomListDto
        {
            Id = classroom.Id,
            SchoolId = classroom.SchoolId,
            Grade = classroom.Grade,
            Section = classroom.Section,
            TeacherId = classroom.TeacherId,
            TeacherName = teacher != null
                ? $"{teacher.FirstName} {teacher.LastName}"
                : null,
            IsActive = classroom.IsActive
        };
    }

    public async Task CreateAsync(CreateClassroomDto dto)
    {
        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);
        if (school is null)
            throw new Exception("Okul bulunamadı.");

        if (dto.TeacherId != null)
        {
            var teacher = await _teacherRepository.GetByIdAsync(dto.TeacherId);
            if (teacher is null)
                throw new Exception("Öğretmen bulunamadı.");
        }

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

        if (dto.TeacherId != null)
        {
            var teacher = await _teacherRepository.GetByIdAsync(dto.TeacherId);
            if (teacher is null)
                throw new Exception("Öğretmen bulunamadı.");
        }

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