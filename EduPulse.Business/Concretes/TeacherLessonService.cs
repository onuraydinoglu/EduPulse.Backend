using EduPulse.Business.Abstracts;
using EduPulse.DTOs.TeacherLessons;
using EduPulse.Entities.TeacherLessons;
using EduPulse.Repository.Abstracts;
using FluentValidation;

namespace EduPulse.Business.Concretes;

public class TeacherLessonService : ITeacherLessonService
{
    private readonly ITeacherLessonRepository _teacherLessonRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IClassroomRepository _classroomRepository;
    private readonly ISchoolRepository _schoolRepository;
    private readonly IValidator<CreateTeacherLessonDto> _createValidator;
    private readonly IValidator<UpdateTeacherLessonDto> _updateValidator;

    public TeacherLessonService(
        ITeacherLessonRepository teacherLessonRepository,
        ITeacherRepository teacherRepository,
        ILessonRepository lessonRepository,
        IClassroomRepository classroomRepository,
        ISchoolRepository schoolRepository,
        IValidator<CreateTeacherLessonDto> createValidator,
        IValidator<UpdateTeacherLessonDto> updateValidator)
    {
        _teacherLessonRepository = teacherLessonRepository;
        _teacherRepository = teacherRepository;
        _lessonRepository = lessonRepository;
        _classroomRepository = classroomRepository;
        _schoolRepository = schoolRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<List<TeacherLessonListDto>> GetAllAsync()
    {
        var list = await _teacherLessonRepository.GetAllAsync();
        var teachers = await _teacherRepository.GetAllAsync();
        var lessons = await _lessonRepository.GetAllAsync();
        var classrooms = await _classroomRepository.GetAllAsync();

        return list.Select(x =>
        {
            var teacher = teachers.FirstOrDefault(t => t.Id == x.TeacherId);
            var lesson = lessons.FirstOrDefault(l => l.Id == x.LessonId);
            var classroom = classrooms.FirstOrDefault(c => c.Id == x.ClassroomId);

            return new TeacherLessonListDto
            {
                Id = x.Id,
                SchoolId = x.SchoolId,
                TeacherId = x.TeacherId,
                TeacherName = teacher != null ? $"{teacher.FirstName} {teacher.LastName}" : "",
                LessonId = x.LessonId,
                LessonName = lesson?.Name ?? "",
                ClassroomId = x.ClassroomId,
                ClassroomName = classroom != null ? $"{classroom.Grade}-{classroom.Section}" : "",
                IsActive = x.IsActive
            };
        }).ToList();
    }

    public async Task<List<TeacherLessonListDto>> GetBySchoolIdAsync(string schoolId)
    {
        var list = await _teacherLessonRepository.GetBySchoolIdAsync(schoolId);
        var teachers = await _teacherRepository.GetAllAsync();
        var lessons = await _lessonRepository.GetAllAsync();
        var classrooms = await _classroomRepository.GetAllAsync();

        return list.Select(x =>
        {
            var teacher = teachers.FirstOrDefault(t => t.Id == x.TeacherId);
            var lesson = lessons.FirstOrDefault(l => l.Id == x.LessonId);
            var classroom = classrooms.FirstOrDefault(c => c.Id == x.ClassroomId);

            return new TeacherLessonListDto
            {
                Id = x.Id,
                SchoolId = x.SchoolId,
                TeacherId = x.TeacherId,
                TeacherName = teacher != null ? $"{teacher.FirstName} {teacher.LastName}" : "",
                LessonId = x.LessonId,
                LessonName = lesson?.Name ?? "",
                ClassroomId = x.ClassroomId,
                ClassroomName = classroom != null ? $"{classroom.Grade}-{classroom.Section}" : "",
                IsActive = x.IsActive
            };
        }).ToList();
    }

    public async Task<List<TeacherLessonListDto>> GetByTeacherIdAsync(string teacherId)
    {
        var list = await _teacherLessonRepository.GetByTeacherIdAsync(teacherId);
        var teachers = await _teacherRepository.GetAllAsync();
        var lessons = await _lessonRepository.GetAllAsync();
        var classrooms = await _classroomRepository.GetAllAsync();

        return list.Select(x =>
        {
            var teacher = teachers.FirstOrDefault(t => t.Id == x.TeacherId);
            var lesson = lessons.FirstOrDefault(l => l.Id == x.LessonId);
            var classroom = classrooms.FirstOrDefault(c => c.Id == x.ClassroomId);

            return new TeacherLessonListDto
            {
                Id = x.Id,
                SchoolId = x.SchoolId,
                TeacherId = x.TeacherId,
                TeacherName = teacher != null ? $"{teacher.FirstName} {teacher.LastName}" : "",
                LessonId = x.LessonId,
                LessonName = lesson?.Name ?? "",
                ClassroomId = x.ClassroomId,
                ClassroomName = classroom != null ? $"{classroom.Grade}-{classroom.Section}" : "",
                IsActive = x.IsActive
            };
        }).ToList();
    }

    public async Task<TeacherLessonListDto?> GetByIdAsync(string id)
    {
        var x = await _teacherLessonRepository.GetByIdAsync(id);

        if (x is null)
            return null;

        var teacher = await _teacherRepository.GetByIdAsync(x.TeacherId);
        var lesson = await _lessonRepository.GetByIdAsync(x.LessonId);
        var classroom = await _classroomRepository.GetByIdAsync(x.ClassroomId);

        return new TeacherLessonListDto
        {
            Id = x.Id,
            SchoolId = x.SchoolId,
            TeacherId = x.TeacherId,
            TeacherName = teacher != null ? $"{teacher.FirstName} {teacher.LastName}" : "",
            LessonId = x.LessonId,
            LessonName = lesson?.Name ?? "",
            ClassroomId = x.ClassroomId,
            ClassroomName = classroom != null ? $"{classroom.Grade}-{classroom.Section}" : "",
            IsActive = x.IsActive
        };
    }

    public async Task CreateAsync(CreateTeacherLessonDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            throw new ArgumentException(validationResult.Errors.First().ErrorMessage);

        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);
        if (school is null)
            throw new ArgumentException("Okul bulunamadı.");

        var teacher = await _teacherRepository.GetByIdAsync(dto.TeacherId);
        if (teacher is null)
            throw new ArgumentException("Öğretmen bulunamadı.");

        var lesson = await _lessonRepository.GetByIdAsync(dto.LessonId);
        if (lesson is null)
            throw new ArgumentException("Ders bulunamadı.");

        var classroom = await _classroomRepository.GetByIdAsync(dto.ClassroomId);
        if (classroom is null)
            throw new ArgumentException("Sınıf bulunamadı.");

        if (teacher.SchoolId != dto.SchoolId)
            throw new ArgumentException("Seçilen öğretmen bu okula ait değil.");

        if (lesson.SchoolId != dto.SchoolId)
            throw new ArgumentException("Seçilen ders bu okula ait değil.");

        if (classroom.SchoolId != dto.SchoolId)
            throw new ArgumentException("Seçilen sınıf bu okula ait değil.");

        var entity = new TeacherLesson
        {
            SchoolId = dto.SchoolId,
            TeacherId = dto.TeacherId,
            LessonId = dto.LessonId,
            ClassroomId = dto.ClassroomId,
            IsActive = true
        };

        await _teacherLessonRepository.CreateAsync(entity);
    }

    public async Task UpdateAsync(UpdateTeacherLessonDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            throw new ArgumentException(validationResult.Errors.First().ErrorMessage);

        var entity = await _teacherLessonRepository.GetByIdAsync(dto.Id);
        if (entity is null)
            throw new ArgumentException("Kayıt bulunamadı.");

        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);
        if (school is null)
            throw new ArgumentException("Okul bulunamadı.");

        var teacher = await _teacherRepository.GetByIdAsync(dto.TeacherId);
        if (teacher is null)
            throw new ArgumentException("Öğretmen bulunamadı.");

        var lesson = await _lessonRepository.GetByIdAsync(dto.LessonId);
        if (lesson is null)
            throw new ArgumentException("Ders bulunamadı.");

        var classroom = await _classroomRepository.GetByIdAsync(dto.ClassroomId);
        if (classroom is null)
            throw new ArgumentException("Sınıf bulunamadı.");

        if (teacher.SchoolId != dto.SchoolId)
            throw new ArgumentException("Seçilen öğretmen bu okula ait değil.");

        if (lesson.SchoolId != dto.SchoolId)
            throw new ArgumentException("Seçilen ders bu okula ait değil.");

        if (classroom.SchoolId != dto.SchoolId)
            throw new ArgumentException("Seçilen sınıf bu okula ait değil.");

        entity.SchoolId = dto.SchoolId;
        entity.TeacherId = dto.TeacherId;
        entity.LessonId = dto.LessonId;
        entity.ClassroomId = dto.ClassroomId;
        entity.IsActive = dto.IsActive;

        await _teacherLessonRepository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(string id)
    {
        var entity = await _teacherLessonRepository.GetByIdAsync(id);

        if (entity is null)
            throw new ArgumentException("Kayıt bulunamadı.");

        await _teacherLessonRepository.DeleteAsync(id);
    }
}