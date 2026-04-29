using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Common;
using EduPulse.DTOs.Students;
using EduPulse.Entities.Students;
using EduPulse.Entities.Users;
using EduPulse.Repository.Abstracts;
using FluentValidation;

namespace EduPulse.Business.Concretes;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IClassroomRepository _classroomRepository;
    private readonly IValidator<CreateStudentDto> _createValidator;
    private readonly IValidator<UpdateStudentDto> _updateValidator;

    public StudentService(
        IStudentRepository studentRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IClassroomRepository classroomRepository,
        IValidator<CreateStudentDto> createValidator,
        IValidator<UpdateStudentDto> updateValidator)
    {
        _studentRepository = studentRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _classroomRepository = classroomRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<List<StudentListDto>>> GetAllForCurrentUserAsync(string? currentRoleName, string? currentSchoolId)
    {
        if (currentRoleName == "superadmin")
            return Result<List<StudentListDto>>.Failure("Superadmin öğrenci işlemi yapamaz.", 403);

        if (string.IsNullOrWhiteSpace(currentSchoolId))
            return Result<List<StudentListDto>>.Failure("Okul bilgisi bulunamadı.", 400);

        var students = await _studentRepository.GetBySchoolIdAsync(currentSchoolId);
        var result = new List<StudentListDto>();

        foreach (var student in students)
        {
            var user = await _userRepository.GetByIdAsync(student.UserId);
            if (user is null) continue;

            var classroom = await _classroomRepository.GetByIdAsync(student.ClassroomId);

            var dto = MapToListDto(student, user);

            if (classroom is not null)
            {
                dto.ClassroomName = $"{classroom.Grade}/{classroom.Section}";
            }

            result.Add(dto);
        }

        return Result<List<StudentListDto>>.Success(result, "Öğrenciler başarıyla listelendi.");
    }

    public async Task<Result<StudentListDto>> GetByIdForCurrentUserAsync(string id, string? currentRoleName, string? currentSchoolId)
    {
        var student = await _studentRepository.GetByIdAsync(id);

        if (student is null)
            return Result<StudentListDto>.Failure("Öğrenci bulunamadı.", 404);

        if (!CanAccessStudent(student, currentRoleName, currentSchoolId))
            return Result<StudentListDto>.Failure("Bu öğrenciye erişim yetkiniz yok.", 403);

        var user = await _userRepository.GetByIdAsync(student.UserId);

        if (user is null)
            return Result<StudentListDto>.Failure("Öğrenciye bağlı kullanıcı bulunamadı.", 404);

        var classroom = await _classroomRepository.GetByIdAsync(student.ClassroomId);

        var dto = MapToListDto(student, user);

        if (classroom is not null)
        {
            dto.ClassroomName = $"{classroom.Grade}/{classroom.Section}";
        }

        return Result<StudentListDto>.Success(dto, "Öğrenci başarıyla getirildi.");
    }

    public async Task<Result> CreateForCurrentUserAsync(CreateStudentDto dto, string? currentRoleName, string? currentSchoolId)
    {
        if (currentRoleName != "schooladmin")
            return Result.Failure("Sadece okul yöneticisi öğrenci oluşturabilir.", 403);

        if (string.IsNullOrWhiteSpace(currentSchoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var classroom = await _classroomRepository.GetByIdAsync(dto.ClassroomId);
        if (classroom is null)
            return Result.Failure("Sınıf bulunamadı.", 404);

        if (classroom.SchoolId != currentSchoolId)
            return Result.Failure("Seçilen sınıf bu okula ait değil.", 403);

        var existsStudentNumber = await _studentRepository.GetBySchoolIdAndStudentNumberAsync(currentSchoolId, dto.StudentNumber);
        if (existsStudentNumber is not null)
            return Result.Failure("Bu öğrenci numarası bu okulda zaten kullanılıyor.", 400);

        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            var existsEmail = await _userRepository.GetByEmailAsync(dto.Email.Trim());
            if (existsEmail is not null)
                return Result.Failure("Bu e-posta adresi zaten kullanılıyor.", 400);
        }

        var studentRole = await _roleRepository.GetByNameAsync("student");
        if (studentRole is null)
            return Result.Failure("Student rolü bulunamadı.", 404);

        var generatedPassword = GeneratePassword(dto.FirstName);

        var user = new User
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = string.IsNullOrWhiteSpace(dto.Email)
                ? GenerateStudentEmail(dto.FirstName, dto.LastName, dto.StudentNumber)
                : dto.Email.Trim(),
            PhoneNumber = dto.PhoneNumber?.Trim() ?? "",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(generatedPassword),
            RoleId = studentRole.Id,
            RoleName = studentRole.Name,
            SchoolId = currentSchoolId,
            IsActive = true
        };

        await _userRepository.CreateAsync(user);

        try
        {
            var student = new Student
            {
                UserId = user.Id,
                SchoolId = currentSchoolId,
                ClassroomId = dto.ClassroomId,
                StudentNumber = dto.StudentNumber.Trim(),
                IsActive = true
            };

            await _studentRepository.CreateAsync(student);
        }
        catch (Exception ex)
        {
            user.IsActive = false;
            user.UpdatedDate = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            return Result.Failure(
                $"Öğrenci oluşturulurken hata oluştu. Kullanıcı geri alındı. Hata: {ex.Message}",
                500
            );
        }

        return Result.Success($"Öğrenci başarıyla oluşturuldu. Geçici şifre: {generatedPassword}", 201);
    }

    public async Task<Result> UpdateForCurrentUserAsync(UpdateStudentDto dto, string? currentRoleName, string? currentSchoolId)
    {
        if (currentRoleName != "schooladmin")
            return Result.Failure("Sadece okul yöneticisi öğrenci güncelleyebilir.", 403);

        if (string.IsNullOrWhiteSpace(currentSchoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var student = await _studentRepository.GetByIdAsync(dto.Id);
        if (student is null)
            return Result.Failure("Öğrenci bulunamadı.", 404);

        if (student.SchoolId != currentSchoolId)
            return Result.Failure("Bu öğrenciyi güncelleme yetkiniz yok.", 403);

        var classroom = await _classroomRepository.GetByIdAsync(dto.ClassroomId);
        if (classroom is null)
            return Result.Failure("Sınıf bulunamadı.", 404);

        if (classroom.SchoolId != currentSchoolId)
            return Result.Failure("Seçilen sınıf bu okula ait değil.", 403);

        var sameNumberStudent = await _studentRepository.GetBySchoolIdAndStudentNumberAsync(currentSchoolId, dto.StudentNumber);
        if (sameNumberStudent is not null && sameNumberStudent.Id != student.Id)
            return Result.Failure("Bu öğrenci numarası bu okulda zaten kullanılıyor.", 400);

        var user = await _userRepository.GetByIdAsync(student.UserId);
        if (user is null)
            return Result.Failure("Öğrenciye bağlı kullanıcı bulunamadı.", 404);

        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            var sameEmailUser = await _userRepository.GetByEmailAsync(dto.Email.Trim());
            if (sameEmailUser is not null && sameEmailUser.Id != user.Id)
                return Result.Failure("Bu e-posta adresi zaten kullanılıyor.", 400);
        }

        user.FirstName = dto.FirstName.Trim();
        user.LastName = dto.LastName.Trim();
        user.Email = string.IsNullOrWhiteSpace(dto.Email)
            ? user.Email
            : dto.Email.Trim();
        user.PhoneNumber = dto.PhoneNumber?.Trim() ?? "";
        user.IsActive = dto.IsActive;
        user.UpdatedDate = DateTime.UtcNow;

        student.ClassroomId = dto.ClassroomId;
        student.StudentNumber = dto.StudentNumber.Trim();
        student.IsActive = dto.IsActive;

        await _userRepository.UpdateAsync(user);
        await _studentRepository.UpdateAsync(student);

        return Result.Success("Öğrenci başarıyla güncellendi.");
    }

    public async Task<Result> DeleteForCurrentUserAsync(string id, string? currentRoleName, string? currentSchoolId)
    {
        if (currentRoleName != "schooladmin")
            return Result.Failure("Sadece okul yöneticisi öğrenci silebilir.", 403);

        if (string.IsNullOrWhiteSpace(currentSchoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        var student = await _studentRepository.GetByIdAsync(id);
        if (student is null)
            return Result.Failure("Öğrenci bulunamadı.", 404);

        if (student.SchoolId != currentSchoolId)
            return Result.Failure("Bu öğrenciyi silme yetkiniz yok.", 403);

        var user = await _userRepository.GetByIdAsync(student.UserId);

        if (user is not null)
        {
            user.IsActive = false;
            user.UpdatedDate = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
        }

        await _studentRepository.DeleteAsync(id);

        return Result.Success("Öğrenci başarıyla silindi.");
    }

    private static bool CanAccessStudent(Student student, string? currentRoleName, string? currentSchoolId)
    {
        if (currentRoleName == "superadmin")
            return false;

        if (currentRoleName == "schooladmin" || currentRoleName == "teacher")
            return student.SchoolId == currentSchoolId;

        return false;
    }

    private static StudentListDto MapToListDto(Student student, User user)
    {
        return new StudentListDto
        {
            Id = student.Id,
            UserId = student.UserId,
            SchoolId = student.SchoolId,
            ClassroomId = student.ClassroomId,
            StudentNumber = student.StudentNumber,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            IsActive = student.IsActive && user.IsActive
        };
    }

    private static string GeneratePassword(string firstName)
    {
        var cleanName = string.IsNullOrWhiteSpace(firstName)
            ? "Ogrenci"
            : firstName.Trim();

        var randomNumber = Random.Shared.Next(1000, 9999);
        return $"{cleanName}{randomNumber}";
    }

    private static string GenerateStudentEmail(string firstName, string lastName, string studentNumber)
    {
        var name = NormalizeForEmail(firstName);
        var surname = NormalizeForEmail(lastName);
        return $"{name}.{surname}.{studentNumber}@student.local";
    }

    private static string NormalizeForEmail(string value)
    {
        return value
            .Trim()
            .ToLower()
            .Replace("ç", "c")
            .Replace("ğ", "g")
            .Replace("ı", "i")
            .Replace("ö", "o")
            .Replace("ş", "s")
            .Replace("ü", "u")
            .Replace(" ", "");
    }
}