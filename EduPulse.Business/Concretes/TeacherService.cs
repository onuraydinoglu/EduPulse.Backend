using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Common;
using EduPulse.DTOs.Teachers;
using EduPulse.Entities.Teachers;
using EduPulse.Entities.Users;
using EduPulse.Repository.Abstracts;
using FluentValidation;

namespace EduPulse.Business.Concretes;

public class TeacherService : ITeacherService
{
    private readonly ITeacherRepository _teacherRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IValidator<CreateTeacherDto> _createValidator;
    private readonly IValidator<UpdateTeacherDto> _updateValidator;

    public TeacherService(
        ITeacherRepository teacherRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ILessonRepository lessonRepository,
        IValidator<CreateTeacherDto> createValidator,
        IValidator<UpdateTeacherDto> updateValidator)
    {
        _teacherRepository = teacherRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _lessonRepository = lessonRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<List<TeacherListDto>>> GetAllForCurrentUserAsync(string? currentRoleName, string? currentSchoolId)
    {
        if (currentRoleName == "superadmin")
            return Result<List<TeacherListDto>>.Failure("Superadmin öğretmen işlemi yapamaz.", 403);

        if (string.IsNullOrWhiteSpace(currentSchoolId))
            return Result<List<TeacherListDto>>.Failure("Okul bilgisi bulunamadı.", 400);

        var teachers = await _teacherRepository.GetBySchoolIdAsync(currentSchoolId);
        var result = new List<TeacherListDto>();

        foreach (var teacher in teachers)
        {
            var user = await _userRepository.GetByIdAsync(teacher.UserId);
            if (user is null) continue;

            var dto = await MapToListDtoAsync(teacher, user);
            result.Add(dto);
        }

        return Result<List<TeacherListDto>>.Success(result, "Öğretmenler başarıyla listelendi.");
    }

    public async Task<Result<TeacherListDto>> GetByIdForCurrentUserAsync(string id, string? currentRoleName, string? currentSchoolId)
    {
        var teacher = await _teacherRepository.GetByIdAsync(id);

        if (teacher is null)
            return Result<TeacherListDto>.Failure("Öğretmen bulunamadı.", 404);

        if (!CanAccessTeacher(teacher, currentRoleName, currentSchoolId))
            return Result<TeacherListDto>.Failure("Bu öğretmene erişim yetkiniz yok.", 403);

        var user = await _userRepository.GetByIdAsync(teacher.UserId);

        if (user is null)
            return Result<TeacherListDto>.Failure("Öğretmene bağlı kullanıcı bulunamadı.", 404);

        var dto = await MapToListDtoAsync(teacher, user);

        return Result<TeacherListDto>.Success(dto, "Öğretmen başarıyla getirildi.");
    }

    public async Task<Result> CreateForCurrentUserAsync(CreateTeacherDto dto, string? currentRoleName, string? currentSchoolId)
    {
        if (currentRoleName != "schooladmin")
            return Result.Failure("Sadece okul yöneticisi öğretmen oluşturabilir.", 403);

        if (string.IsNullOrWhiteSpace(currentSchoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        var validationResult = await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var existsEmail = await _userRepository.GetByEmailAsync(dto.Email.Trim());

        if (existsEmail is not null)
            return Result.Failure("Bu e-posta adresi zaten kullanılıyor.", 400);

        if (!string.IsNullOrWhiteSpace(dto.BranchLessonId))
        {
            var lesson = await _lessonRepository.GetByIdAsync(dto.BranchLessonId);

            if (lesson is null)
                return Result.Failure("Seçilen branş dersi bulunamadı.", 404);

            if (lesson.SchoolId != currentSchoolId)
                return Result.Failure("Seçilen branş dersi bu okula ait değil.", 403);
        }

        var teacherRole = await _roleRepository.GetByNameAsync("teacher");

        if (teacherRole is null)
            return Result.Failure("Teacher rolü bulunamadı.", 404);

        var generatedPassword = GeneratePassword(dto.FirstName);

        var user = new User
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.Trim(),
            PhoneNumber = dto.PhoneNumber?.Trim() ?? "",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(generatedPassword),
            RoleId = teacherRole.Id,
            RoleName = teacherRole.Name,
            SchoolId = currentSchoolId,
            IsActive = true
        };

        await _userRepository.CreateAsync(user);

        try
        {
            var teacher = new Teacher
            {
                UserId = user.Id,
                SchoolId = currentSchoolId,
                BranchLessonId = string.IsNullOrWhiteSpace(dto.BranchLessonId) ? null : dto.BranchLessonId,
                Department = string.IsNullOrWhiteSpace(dto.Department) ? null : dto.Department.Trim(),
                IsActive = true
            };

            await _teacherRepository.CreateAsync(teacher);
        }
        catch (Exception ex)
        {
            user.IsActive = false;
            user.UpdatedDate = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            return Result.Failure($"Öğretmen oluşturulurken hata oluştu. Kullanıcı geri alındı. Hata: {ex.Message}", 500);
        }

        return Result.Success($"Öğretmen başarıyla oluşturuldu. Geçici şifre: {generatedPassword}", 201);
    }

    public async Task<Result> UpdateForCurrentUserAsync(UpdateTeacherDto dto, string? currentRoleName, string? currentSchoolId)
    {
        if (currentRoleName != "schooladmin")
            return Result.Failure("Sadece okul yöneticisi öğretmen güncelleyebilir.", 403);

        if (string.IsNullOrWhiteSpace(currentSchoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var teacher = await _teacherRepository.GetByIdAsync(dto.Id);

        if (teacher is null)
            return Result.Failure("Öğretmen bulunamadı.", 404);

        if (teacher.SchoolId != currentSchoolId)
            return Result.Failure("Bu öğretmeni güncelleme yetkiniz yok.", 403);

        var user = await _userRepository.GetByIdAsync(teacher.UserId);

        if (user is null)
            return Result.Failure("Öğretmene bağlı kullanıcı bulunamadı.", 404);

        var sameEmailUser = await _userRepository.GetByEmailAsync(dto.Email.Trim());

        if (sameEmailUser is not null && sameEmailUser.Id != user.Id)
            return Result.Failure("Bu e-posta adresi zaten kullanılıyor.", 400);

        if (!string.IsNullOrWhiteSpace(dto.BranchLessonId))
        {
            var lesson = await _lessonRepository.GetByIdAsync(dto.BranchLessonId);

            if (lesson is null)
                return Result.Failure("Seçilen branş dersi bulunamadı.", 404);

            if (lesson.SchoolId != currentSchoolId)
                return Result.Failure("Seçilen branş dersi bu okula ait değil.", 403);
        }

        user.FirstName = dto.FirstName.Trim();
        user.LastName = dto.LastName.Trim();
        user.Email = dto.Email.Trim();
        user.PhoneNumber = dto.PhoneNumber?.Trim() ?? "";
        user.IsActive = dto.IsActive;
        user.UpdatedDate = DateTime.UtcNow;

        teacher.BranchLessonId = string.IsNullOrWhiteSpace(dto.BranchLessonId) ? null : dto.BranchLessonId;
        teacher.Department = string.IsNullOrWhiteSpace(dto.Department) ? null : dto.Department.Trim();
        teacher.IsActive = dto.IsActive;
        teacher.UpdatedDate = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        await _teacherRepository.UpdateAsync(teacher);

        return Result.Success("Öğretmen başarıyla güncellendi.");
    }

    public async Task<Result> DeleteForCurrentUserAsync(string id, string? currentRoleName, string? currentSchoolId)
    {
        if (currentRoleName != "schooladmin")
            return Result.Failure("Sadece okul yöneticisi öğretmen silebilir.", 403);

        if (string.IsNullOrWhiteSpace(currentSchoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        var teacher = await _teacherRepository.GetByIdAsync(id);

        if (teacher is null)
            return Result.Failure("Öğretmen bulunamadı.", 404);

        if (teacher.SchoolId != currentSchoolId)
            return Result.Failure("Bu öğretmeni silme yetkiniz yok.", 403);

        var user = await _userRepository.GetByIdAsync(teacher.UserId);

        if (user is not null)
        {
            user.IsActive = false;
            user.UpdatedDate = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
        }

        await _teacherRepository.DeleteAsync(id);

        return Result.Success("Öğretmen başarıyla silindi.");
    }

    private async Task<TeacherListDto> MapToListDtoAsync(Teacher teacher, User user)
    {
        string? branchLessonName = null;

        if (!string.IsNullOrWhiteSpace(teacher.BranchLessonId))
        {
            var lesson = await _lessonRepository.GetByIdAsync(teacher.BranchLessonId);

            if (lesson is not null)
                branchLessonName = lesson.Name;
        }

        return new TeacherListDto
        {
            Id = teacher.Id,
            UserId = teacher.UserId,
            SchoolId = teacher.SchoolId,

            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = $"{user.FirstName} {user.LastName}",

            Email = user.Email,
            PhoneNumber = user.PhoneNumber,

            BranchLessonId = teacher.BranchLessonId,
            BranchLessonName = branchLessonName,
            Department = teacher.Department,

            IsActive = teacher.IsActive && user.IsActive
        };
    }

    private static bool CanAccessTeacher(Teacher teacher, string? currentRoleName, string? currentSchoolId)
    {
        if (currentRoleName == "superadmin")
            return false;

        if (currentRoleName == "schooladmin")
            return teacher.SchoolId == currentSchoolId;

        return false;
    }

    private static string GeneratePassword(string firstName)
    {
        var cleanName = string.IsNullOrWhiteSpace(firstName)
            ? "Ogretmen"
            : firstName.Trim();

        var randomNumber = Random.Shared.Next(1000, 9999);

        return $"{cleanName}{randomNumber}";
    }
}