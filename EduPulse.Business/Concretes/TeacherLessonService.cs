using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Common;
using EduPulse.DTOs.TeacherLessons;
using EduPulse.Entities.TeacherLessons;
using EduPulse.Repository.Abstracts;

namespace EduPulse.Business.Concretes;

public class TeacherLessonService : ITeacherLessonService
{
    private readonly ITeacherLessonRepository _teacherLessonRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IClassroomRepository _classroomRepository;
    private readonly IUserRepository _userRepository;

    public TeacherLessonService(
        ITeacherLessonRepository teacherLessonRepository,
        ITeacherRepository teacherRepository,
        ILessonRepository lessonRepository,
        IClassroomRepository classroomRepository,
        IUserRepository userRepository)
    {
        _teacherLessonRepository = teacherLessonRepository;
        _teacherRepository = teacherRepository;
        _lessonRepository = lessonRepository;
        _classroomRepository = classroomRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<List<TeacherLessonListDto>>> GetAllForCurrentUserAsync(
    string? roleName,
    string? schoolId)
    {
        if (roleName != "superadmin" && string.IsNullOrWhiteSpace(schoolId))
            return Result<List<TeacherLessonListDto>>.Failure("Okul bilgisi bulunamadı.", 400);

        var teacherLessons = roleName == "superadmin"
            ? await _teacherLessonRepository.GetAllAsync()
            : await _teacherLessonRepository.GetBySchoolIdAsync(schoolId!);

        var teachers = await _teacherRepository.GetAllAsync();
        var lessons = await _lessonRepository.GetAllAsync();
        var classrooms = await _classroomRepository.GetAllAsync();

        var result = new List<TeacherLessonListDto>();

        foreach (var x in teacherLessons)
        {
            var teacher = teachers.FirstOrDefault(t => t.Id == x.TeacherId);
            var lesson = lessons.FirstOrDefault(l => l.Id == x.LessonId);
            var classroom = classrooms.FirstOrDefault(c => c.Id == x.ClassroomId);

            var user = teacher != null
                ? await _userRepository.GetByIdAsync(teacher.UserId)
                : null;

            result.Add(new TeacherLessonListDto
            {
                Id = x.Id,
                SchoolId = x.SchoolId,

                TeacherId = x.TeacherId,
                TeacherFullName = user != null
                    ? $"{user.FirstName} {user.LastName}"
                    : "-",

                LessonId = x.LessonId,
                LessonName = lesson?.Name ?? "-",

                ClassroomId = x.ClassroomId,
                ClassroomName = classroom != null
                    ? $"{classroom.Grade}-{classroom.Section}"
                    : "-",

                IsActive = x.IsActive
            });
        }

        return Result<List<TeacherLessonListDto>>.Success(result);
    }

    public async Task<Result<TeacherLessonListDto>> GetByIdForCurrentUserAsync(
    string id,
    string? roleName,
    string? schoolId)
    {
        var teacherLesson = await _teacherLessonRepository.GetByIdAsync(id);

        if (teacherLesson == null)
            return Result<TeacherLessonListDto>.Failure("Kayıt bulunamadı.", 404);

        if (roleName != "superadmin")
        {
            if (string.IsNullOrWhiteSpace(schoolId))
                return Result<TeacherLessonListDto>.Failure("Okul bilgisi bulunamadı.", 400);

            if (teacherLesson.SchoolId != schoolId)
                return Result<TeacherLessonListDto>.Failure("Bu kayda erişim yetkiniz yok.", 403);
        }

        var teacher = await _teacherRepository.GetByIdAsync(teacherLesson.TeacherId);
        var lesson = await _lessonRepository.GetByIdAsync(teacherLesson.LessonId);
        var classroom = await _classroomRepository.GetByIdAsync(teacherLesson.ClassroomId);

        var user = teacher != null
            ? await _userRepository.GetByIdAsync(teacher.UserId)
            : null;

        var dto = new TeacherLessonListDto
        {
            Id = teacherLesson.Id,
            SchoolId = teacherLesson.SchoolId,

            TeacherId = teacherLesson.TeacherId,
            TeacherFullName = user != null
                ? $"{user.FirstName} {user.LastName}"
                : "-",

            LessonId = teacherLesson.LessonId,
            LessonName = lesson?.Name ?? "-",

            ClassroomId = teacherLesson.ClassroomId,
            ClassroomName = classroom != null
                ? $"{classroom.Grade}-{classroom.Section}"
                : "-",

            IsActive = teacherLesson.IsActive
        };

        return Result<TeacherLessonListDto>.Success(dto);
    }

    public async Task<Result> CreateAsync(CreateTeacherLessonDto dto, string? schoolId)
    {
        if (string.IsNullOrWhiteSpace(schoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        var classroom = await _classroomRepository.GetByIdAsync(dto.ClassroomId);
        if (classroom == null || classroom.SchoolId != schoolId)
            return Result.Failure("Sınıf bulunamadı.", 404);

        var lesson = await _lessonRepository.GetByIdAsync(dto.LessonId);
        if (lesson == null || lesson.SchoolId != schoolId)
            return Result.Failure("Ders bulunamadı.", 404);

        var teacher = await _teacherRepository.GetByIdAsync(dto.TeacherId);
        if (teacher == null || teacher.SchoolId != schoolId)
            return Result.Failure("Öğretmen bulunamadı.", 404);

        var duplicate = await _teacherLessonRepository.GetDuplicateAsync(
            schoolId,
            dto.TeacherId,
            dto.LessonId,
            dto.ClassroomId);

        if (duplicate != null)
            return Result.Failure("Bu öğretmen bu sınıfa bu ders için zaten atanmış.", 400);

        var teacherLesson = new TeacherLesson
        {
            SchoolId = schoolId,
            TeacherId = dto.TeacherId,
            LessonId = dto.LessonId,
            ClassroomId = dto.ClassroomId,
            IsActive = true
        };

        await _teacherLessonRepository.AddAsync(teacherLesson);

        return Result.Success("Ders öğretmene ve sınıfa başarıyla bağlandı.");
    }

    public async Task<Result> UpdateAsync(UpdateTeacherLessonDto dto, string? schoolId)
    {
        if (string.IsNullOrWhiteSpace(schoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        var teacherLesson = await _teacherLessonRepository.GetByIdAsync(dto.Id);

        if (teacherLesson == null)
            return Result.Failure("Kayıt bulunamadı.", 404);

        if (teacherLesson.SchoolId != schoolId)
            return Result.Failure("Bu kaydı güncelleme yetkiniz yok.", 403);

        var classroom = await _classroomRepository.GetByIdAsync(dto.ClassroomId);
        if (classroom == null || classroom.SchoolId != schoolId)    
            return Result.Failure("Sınıf bulunamadı.", 404);

        var lesson = await _lessonRepository.GetByIdAsync(dto.LessonId);
        if (lesson == null || lesson.SchoolId != schoolId)
            return Result.Failure("Ders bulunamadı.", 404);

        var teacher = await _teacherRepository.GetByIdAsync(dto.TeacherId);
        if (teacher == null || teacher.SchoolId != schoolId)
            return Result.Failure("Öğretmen bulunamadı.", 404);

        var duplicate = await _teacherLessonRepository.GetDuplicateAsync(
            schoolId,
            dto.TeacherId,
            dto.LessonId,
            dto.ClassroomId);

        if (duplicate != null && duplicate.Id != dto.Id)
            return Result.Failure("Bu öğretmen bu sınıfta bu derse zaten atanmış.", 400);

        teacherLesson.TeacherId = dto.TeacherId;
        teacherLesson.LessonId = dto.LessonId;
        teacherLesson.ClassroomId = dto.ClassroomId;

        await _teacherLessonRepository.UpdateAsync(teacherLesson);

        return Result.Success("Ders-öğretmen-sınıf bağlantısı güncellendi.");
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var teacherLesson = await _teacherLessonRepository.GetByIdAsync(id);

        if (teacherLesson == null)
            return Result.Failure("Kayıt bulunamadı.", 404);

        await _teacherLessonRepository.DeleteAsync(id);

        return Result.Success("Bağlantı silindi.");
    }
}