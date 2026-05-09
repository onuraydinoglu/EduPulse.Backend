using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Common;
using EduPulse.DTOs.TeacherLessons;
using EduPulse.Entities.TeacherLessons;
using EduPulse.Repository.Abstracts;
using EduPulse.Repository.Concretes;

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
        string? schoolId,
        string? currentUserId)
    {
        if (roleName != "superadmin" && string.IsNullOrWhiteSpace(schoolId))
            return Result<List<TeacherLessonListDto>>.Failure("Okul bilgisi bulunamadı.", 400);

        List<TeacherLesson> teacherLessons;

        if (roleName == "superadmin")
        {
            teacherLessons = await _teacherLessonRepository.GetAllAsync();
        }
        else if (roleName == "teacher")
        {
            if (string.IsNullOrWhiteSpace(currentUserId))
                return Result<List<TeacherLessonListDto>>.Failure("Kullanıcı bilgisi bulunamadı.", 400);

            var teacher = await _teacherRepository.GetByUserIdAsync(currentUserId);

            if (teacher is null)
                return Result<List<TeacherLessonListDto>>.Failure("Öğretmen kaydı bulunamadı.", 404);

            if (teacher.SchoolId != schoolId)
                return Result<List<TeacherLessonListDto>>.Failure("Bu kayıtlara erişim yetkiniz yok.", 403);

            teacherLessons = await _teacherLessonRepository.GetByTeacherIdAsync(teacher.Id);
        }
        else
        {
            teacherLessons = await _teacherLessonRepository.GetBySchoolIdAsync(schoolId!);
        }

        var result = new List<TeacherLessonListDto>();

        foreach (var teacherLesson in teacherLessons)
        {
            result.Add(await MapToDtoAsync(teacherLesson));
        }

        return Result<List<TeacherLessonListDto>>.Success(
            result,
            "Ders atamaları başarıyla listelendi.",
            200
        );
    }

    public async Task<Result<TeacherLessonListDto>> GetByIdForCurrentUserAsync(
        string id,
        string? roleName,
        string? schoolId,
        string? currentUserId)
    {
        var teacherLesson = await _teacherLessonRepository.GetByIdAsync(id);

        if (teacherLesson is null)
            return Result<TeacherLessonListDto>.Failure("Kayıt bulunamadı.", 404);

        if (roleName != "superadmin")
        {
            if (string.IsNullOrWhiteSpace(schoolId))
                return Result<TeacherLessonListDto>.Failure("Okul bilgisi bulunamadı.", 400);

            if (teacherLesson.SchoolId != schoolId)
                return Result<TeacherLessonListDto>.Failure("Bu kayda erişim yetkiniz yok.", 403);
        }

        if (roleName == "teacher")
        {
            if (string.IsNullOrWhiteSpace(currentUserId))
                return Result<TeacherLessonListDto>.Failure("Kullanıcı bilgisi bulunamadı.", 400);

            var teacher = await _teacherRepository.GetByUserIdAsync(currentUserId);

            if (teacher is null)
                return Result<TeacherLessonListDto>.Failure("Öğretmen kaydı bulunamadı.", 404);

            if (teacherLesson.TeacherId != teacher.Id)
                return Result<TeacherLessonListDto>.Failure("Bu kayda erişim yetkiniz yok.", 403);
        }

        var dto = await MapToDtoAsync(teacherLesson);

        return Result<TeacherLessonListDto>.Success(
            dto,
            "Ders ataması başarıyla getirildi.",
            200
        );
    }

    public async Task<Result> CreateAsync(CreateTeacherLessonDto dto, string? schoolId)
    {
        if (string.IsNullOrWhiteSpace(schoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        var classroom = await _classroomRepository.GetByIdAsync(dto.ClassroomId);

        if (classroom is null || classroom.SchoolId != schoolId)
            return Result.Failure("Sınıf bulunamadı.", 404);

        var lesson = await _lessonRepository.GetByIdAsync(dto.LessonId);

        if (lesson is null || lesson.SchoolId != schoolId)
            return Result.Failure("Ders bulunamadı.", 404);

        var teacher = await _teacherRepository.GetByIdAsync(dto.TeacherId);

        if (teacher is null || teacher.SchoolId != schoolId)
            return Result.Failure("Öğretmen bulunamadı.", 404);

        var duplicate = await _teacherLessonRepository.GetDuplicateAsync(
            schoolId,
            dto.TeacherId,
            dto.LessonId,
            dto.ClassroomId
        );

        if (duplicate is not null)
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

        return Result.Success("Ders öğretmene ve sınıfa başarıyla bağlandı.", 201);
    }

    public async Task<Result> UpdateAsync(UpdateTeacherLessonDto dto, string? schoolId)
    {
        if (string.IsNullOrWhiteSpace(schoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        var teacherLesson = await _teacherLessonRepository.GetByIdAsync(dto.Id);

        if (teacherLesson is null)
            return Result.Failure("Kayıt bulunamadı.", 404);

        if (teacherLesson.SchoolId != schoolId)
            return Result.Failure("Bu kaydı güncelleme yetkiniz yok.", 403);

        var classroom = await _classroomRepository.GetByIdAsync(dto.ClassroomId);

        if (classroom is null || classroom.SchoolId != schoolId)
            return Result.Failure("Sınıf bulunamadı.", 404);

        var lesson = await _lessonRepository.GetByIdAsync(dto.LessonId);

        if (lesson is null || lesson.SchoolId != schoolId)
            return Result.Failure("Ders bulunamadı.", 404);

        var teacher = await _teacherRepository.GetByIdAsync(dto.TeacherId);

        if (teacher is null || teacher.SchoolId != schoolId)
            return Result.Failure("Öğretmen bulunamadı.", 404);

        var duplicate = await _teacherLessonRepository.GetDuplicateAsync(
            schoolId,
            dto.TeacherId,
            dto.LessonId,
            dto.ClassroomId
        );

        if (duplicate is not null && duplicate.Id != dto.Id)
            return Result.Failure("Bu öğretmen bu sınıfta bu derse zaten atanmış.", 400);

        teacherLesson.TeacherId = dto.TeacherId;
        teacherLesson.LessonId = dto.LessonId;
        teacherLesson.ClassroomId = dto.ClassroomId;
        teacherLesson.IsActive = dto.IsActive;

        await _teacherLessonRepository.UpdateAsync(teacherLesson);

        return Result.Success("Ders-öğretmen-sınıf bağlantısı güncellendi.", 200);
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var teacherLesson = await _teacherLessonRepository.GetByIdAsync(id);

        if (teacherLesson is null)
            return Result.Failure("Kayıt bulunamadı.", 404);

        await _teacherLessonRepository.DeleteAsync(id);

        return Result.Success("Bağlantı silindi.", 200);
    }

    private async Task<TeacherLessonListDto> MapToDtoAsync(TeacherLesson teacherLesson)
    {
        var teacher = await _teacherRepository.GetByIdAsync(teacherLesson.TeacherId);
        var lesson = await _lessonRepository.GetByIdAsync(teacherLesson.LessonId);
        var classroom = await _classroomRepository.GetByIdAsync(teacherLesson.ClassroomId);
        var user = teacher is not null
            ? await _userRepository.GetByIdAsync(teacher.UserId)
            : null;

        return new TeacherLessonListDto
        {
            Id = teacherLesson.Id,
            SchoolId = teacherLesson.SchoolId,
            TeacherId = teacherLesson.TeacherId,
            TeacherFullName = user is not null ? $"{user.FirstName} {user.LastName}" : "-",
            LessonId = teacherLesson.LessonId,
            LessonName = lesson?.Name ?? "-",
            ClassroomId = teacherLesson.ClassroomId,
            ClassroomName = classroom is not null ? $"{classroom.Grade}-{classroom.Section}" : "-",
            IsActive = teacherLesson.IsActive
        };
    }
}