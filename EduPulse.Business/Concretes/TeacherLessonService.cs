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
            var dto = await MapToDtoAsync(teacherLesson);
            result.Add(dto);
        }

        var orderedResult = result
            .OrderBy(x => x.TeacherFullName, StringComparer.Create(new System.Globalization.CultureInfo("tr-TR"), false))
            .ThenBy(x => x.LessonName, StringComparer.Create(new System.Globalization.CultureInfo("tr-TR"), false))
            .ThenBy(x => x.ClassroomName, StringComparer.Create(new System.Globalization.CultureInfo("tr-TR"), false))
            .ToList();

        return Result<List<TeacherLessonListDto>>.Success(
            orderedResult,
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

    public async Task<Result<string>> CreateAsync(CreateTeacherLessonDto dto, string? schoolId)
    {
        if (string.IsNullOrWhiteSpace(schoolId))
            return Result<string>.Failure("Okul bilgisi bulunamadı.", 400);

        if (string.IsNullOrWhiteSpace(dto.TeacherId))
            return Result<string>.Failure("Öğretmen seçimi zorunludur.", 400);

        if (string.IsNullOrWhiteSpace(dto.LessonId))
            return Result<string>.Failure("Ders seçimi zorunludur.", 400);

        var selectedClassroomIds = new List<string>();

        if (dto.ClassroomIds is not null && dto.ClassroomIds.Any())
        {
            selectedClassroomIds = dto.ClassroomIds
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct()
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(dto.ClassroomId) &&
            !selectedClassroomIds.Contains(dto.ClassroomId))
        {
            selectedClassroomIds.Add(dto.ClassroomId);
        }

        if (!selectedClassroomIds.Any())
            return Result<string>.Failure("En az bir sınıf seçilmelidir.", 400);

        var lesson = await _lessonRepository.GetByIdAsync(dto.LessonId);

        if (lesson is null || lesson.SchoolId != schoolId)
            return Result<string>.Failure("Ders bulunamadı.", 404);

        var teacher = await _teacherRepository.GetByIdAsync(dto.TeacherId);

        if (teacher is null || teacher.SchoolId != schoolId)
            return Result<string>.Failure("Öğretmen bulunamadı.", 404);

        var createdCount = 0;
        var skippedCount = 0;

        foreach (var classroomId in selectedClassroomIds)
        {
            var classroom = await _classroomRepository.GetByIdAsync(classroomId);

            if (classroom is null || classroom.SchoolId != schoolId)
                return Result<string>.Failure("Seçilen sınıflardan biri bulunamadı.", 404);

            var duplicate = await _teacherLessonRepository.GetDuplicateAsync(
                schoolId,
                dto.TeacherId,
                dto.LessonId,
                classroomId
            );

            if (duplicate is not null)
            {
                skippedCount++;
                continue;
            }

            var teacherLesson = new TeacherLesson
            {
                SchoolId = schoolId,
                TeacherId = dto.TeacherId,
                LessonId = dto.LessonId,
                ClassroomId = classroomId
            };

            await _teacherLessonRepository.AddAsync(teacherLesson);
            createdCount++;
        }

        if (createdCount == 0)
            return Result<string>.Failure("Seçilen sınıfların tamamı için bu atama zaten mevcut.", 400);

        var messageParts = new List<string>();

        if (createdCount > 0)
            messageParts.Add($"{createdCount} sınıf için yeni ders ataması oluşturuldu.");

        if (skippedCount > 0)
            messageParts.Add($"{skippedCount} mevcut atama tekrar eklenmedi.");

        var message = string.Join(" ", messageParts);

        return Result<string>.Success(
            message,
            message,
            201
        );
    }

    public async Task<Result<string>> UpdateAsync(UpdateTeacherLessonDto dto, string? schoolId)
    {
        if (string.IsNullOrWhiteSpace(schoolId))
            return Result<string>.Failure("Okul bilgisi bulunamadı.", 400);

        if (string.IsNullOrWhiteSpace(dto.Id))
            return Result<string>.Failure("Kayıt id bilgisi zorunludur.", 400);

        if (string.IsNullOrWhiteSpace(dto.TeacherId))
            return Result<string>.Failure("Öğretmen seçimi zorunludur.", 400);

        if (string.IsNullOrWhiteSpace(dto.LessonId))
            return Result<string>.Failure("Ders seçimi zorunludur.", 400);

        if (string.IsNullOrWhiteSpace(dto.ClassroomId))
            return Result<string>.Failure("Sınıf seçimi zorunludur.", 400);

        var teacherLesson = await _teacherLessonRepository.GetByIdAsync(dto.Id);

        if (teacherLesson is null)
            return Result<string>.Failure("Kayıt bulunamadı.", 404);

        if (teacherLesson.SchoolId != schoolId)
            return Result<string>.Failure("Bu kaydı güncelleme yetkiniz yok.", 403);

        var classroom = await _classroomRepository.GetByIdAsync(dto.ClassroomId);

        if (classroom is null || classroom.SchoolId != schoolId)
            return Result<string>.Failure("Sınıf bulunamadı.", 404);

        var lesson = await _lessonRepository.GetByIdAsync(dto.LessonId);

        if (lesson is null || lesson.SchoolId != schoolId)
            return Result<string>.Failure("Ders bulunamadı.", 404);

        var teacher = await _teacherRepository.GetByIdAsync(dto.TeacherId);

        if (teacher is null || teacher.SchoolId != schoolId)
            return Result<string>.Failure("Öğretmen bulunamadı.", 404);

        var duplicate = await _teacherLessonRepository.GetDuplicateAsync(
            schoolId,
            dto.TeacherId,
            dto.LessonId,
            dto.ClassroomId
        );

        if (duplicate is not null && duplicate.Id != dto.Id)
            return Result<string>.Failure("Bu öğretmen bu sınıfta bu derse zaten atanmış.", 400);

        teacherLesson.TeacherId = dto.TeacherId;
        teacherLesson.LessonId = dto.LessonId;
        teacherLesson.ClassroomId = dto.ClassroomId;

        await _teacherLessonRepository.UpdateAsync(teacherLesson);

        return Result<string>.Success(
            "Ders ataması başarıyla güncellendi.",
            "Ders ataması başarıyla güncellendi.",
            200
        );
    }

    public async Task<Result<string>> DeleteAsync(string id)
    {
        var teacherLesson = await _teacherLessonRepository.GetByIdAsync(id);

        if (teacherLesson is null)
            return Result<string>.Failure("Kayıt bulunamadı.", 404);

        await _teacherLessonRepository.DeleteAsync(id);

        return Result<string>.Success(
            "Ders ataması başarıyla silindi.",
            "Ders ataması başarıyla silindi.",
            200
        );
    }

    public async Task<Result<string>> DeleteSelectedLessonAssignmentsAsync(string id, string? schoolId)
    {
        if (string.IsNullOrWhiteSpace(schoolId))
            return Result<string>.Failure("Okul bilgisi bulunamadı.", 400);

        if (string.IsNullOrWhiteSpace(id))
            return Result<string>.Failure("Silinecek kayıt bilgisi zorunludur.", 400);

        var selectedTeacherLesson = await _teacherLessonRepository.GetByIdAsync(id);

        if (selectedTeacherLesson is null)
            return Result<string>.Failure("Kayıt bulunamadı.", 404);

        if (selectedTeacherLesson.SchoolId != schoolId)
            return Result<string>.Failure("Bu kaydı silme yetkiniz yok.", 403);

        var deletedCount = await _teacherLessonRepository.DeleteByTeacherAndLessonAsync(
            selectedTeacherLesson.SchoolId,
            selectedTeacherLesson.TeacherId,
            selectedTeacherLesson.LessonId
        );

        if (deletedCount == 0)
            return Result<string>.Failure("Silinecek ders ataması bulunamadı.", 404);

        var message = $"{deletedCount} ders ataması başarıyla silindi.";

        return Result<string>.Success(message, message, 200);
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
            TeacherFullName = user is not null
                ? $"{user.FirstName} {user.LastName}"
                : "-",

            LessonId = teacherLesson.LessonId,
            LessonName = lesson?.Name ?? "-",

            ClassroomId = teacherLesson.ClassroomId,
            ClassroomName = classroom is not null
                ? $"{classroom.Grade}-{classroom.Section}"
                : "-"
        };
    }
}