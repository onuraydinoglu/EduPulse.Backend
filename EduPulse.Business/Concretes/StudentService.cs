using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Common;
using EduPulse.DTOs.Students;
using EduPulse.Entities.Students;
using EduPulse.Repository.Abstracts;
using FluentValidation;

namespace EduPulse.Business.Concretes;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly ISchoolRepository _schoolRepository;
    private readonly IClassroomRepository _classroomRepository;
    private readonly IValidator<CreateStudentDto> _createValidator;
    private readonly IValidator<UpdateStudentDto> _updateValidator;

    public StudentService(
        IStudentRepository studentRepository,
        ISchoolRepository schoolRepository,
        IClassroomRepository classroomRepository,
        IValidator<CreateStudentDto> createValidator,
        IValidator<UpdateStudentDto> updateValidator)
    {
        _studentRepository = studentRepository;
        _schoolRepository = schoolRepository;
        _classroomRepository = classroomRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<List<StudentListDto>>> GetAllAsync()
    {
        var students = await _studentRepository.GetAllAsync();

        var result = students.Select(x => new StudentListDto
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName,
            SchoolNumber = x.SchoolNumber,
            StudentPhone = x.StudentPhone,
            SchoolId = x.SchoolId,
            ClassroomId = x.ClassroomId,
            ClubIds = x.ClubIds,
            ParentIds = x.ParentIds,
            IsActive = x.IsActive
        }).ToList();

        return Result<List<StudentListDto>>.Success(result, "Öğrenciler başarıyla listelendi.");
    }

    public async Task<Result<List<StudentListDto>>> GetBySchoolIdAsync(string schoolId)
    {
        var school = await _schoolRepository.GetByIdAsync(schoolId);

        if (school is null)
            return Result<List<StudentListDto>>.Failure("Okul bulunamadı.", 404);

        var students = await _studentRepository.GetBySchoolIdAsync(schoolId);

        var result = students.Select(x => new StudentListDto
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName,
            SchoolNumber = x.SchoolNumber,
            StudentPhone = x.StudentPhone,
            SchoolId = x.SchoolId,
            ClassroomId = x.ClassroomId,
            ClubIds = x.ClubIds,
            ParentIds = x.ParentIds,
            IsActive = x.IsActive
        }).ToList();

        return Result<List<StudentListDto>>.Success(result, "Okula ait öğrenciler başarıyla listelendi.");
    }

    public async Task<Result<List<StudentListDto>>> GetByClassroomIdAsync(string classroomId)
    {
        var classroom = await _classroomRepository.GetByIdAsync(classroomId);

        if (classroom is null)
            return Result<List<StudentListDto>>.Failure("Sınıf bulunamadı.", 404);

        var students = await _studentRepository.GetByClassroomIdAsync(classroomId);

        var result = students.Select(x => new StudentListDto
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName,
            SchoolNumber = x.SchoolNumber,
            StudentPhone = x.StudentPhone,
            SchoolId = x.SchoolId,
            ClassroomId = x.ClassroomId,
            ClubIds = x.ClubIds,
            ParentIds = x.ParentIds,
            IsActive = x.IsActive
        }).ToList();

        return Result<List<StudentListDto>>.Success(result, "Sınıfa ait öğrenciler başarıyla listelendi.");
    }

    public async Task<Result<StudentListDto>> GetByIdAsync(string id)
    {
        var student = await _studentRepository.GetByIdAsync(id);

        if (student is null)
            return Result<StudentListDto>.Failure("Öğrenci bulunamadı.", 404);

        var result = new StudentListDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            SchoolNumber = student.SchoolNumber,
            StudentPhone = student.StudentPhone,
            SchoolId = student.SchoolId,
            ClassroomId = student.ClassroomId,
            ClubIds = student.ClubIds,
            ParentIds = student.ParentIds,
            IsActive = student.IsActive
        };

        return Result<StudentListDto>.Success(result, "Öğrenci başarıyla getirildi.");
    }

    public async Task<Result> CreateAsync(CreateStudentDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);

        if (school is null)
            return Result.Failure("Okul bulunamadı.", 404);

        var classroom = await _classroomRepository.GetByIdAsync(dto.ClassroomId);

        if (classroom is null)
            return Result.Failure("Sınıf bulunamadı.", 404);

        var student = new Student
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            SchoolNumber = dto.SchoolNumber,
            StudentPhone = dto.StudentPhone,
            SchoolId = dto.SchoolId,
            ClassroomId = dto.ClassroomId,
            ClubIds = dto.ClubIds,
            ParentIds = dto.ParentIds,
            IsActive = true
        };

        await _studentRepository.CreateAsync(student);

        return Result.Success("Öğrenci başarıyla oluşturuldu.", 201);
    }

    public async Task<Result> UpdateAsync(UpdateStudentDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var student = await _studentRepository.GetByIdAsync(dto.Id);

        if (student is null)
            return Result.Failure("Öğrenci bulunamadı.", 404);

        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);

        if (school is null)
            return Result.Failure("Okul bulunamadı.", 404);

        var classroom = await _classroomRepository.GetByIdAsync(dto.ClassroomId);

        if (classroom is null)
            return Result.Failure("Sınıf bulunamadı.", 404);

        student.FirstName = dto.FirstName;
        student.LastName = dto.LastName;
        student.SchoolNumber = dto.SchoolNumber;
        student.StudentPhone = dto.StudentPhone;
        student.SchoolId = dto.SchoolId;
        student.ClassroomId = dto.ClassroomId;
        student.ClubIds = dto.ClubIds;
        student.ParentIds = dto.ParentIds;
        student.IsActive = dto.IsActive;

        await _studentRepository.UpdateAsync(student);

        return Result.Success("Öğrenci başarıyla güncellendi.");
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var student = await _studentRepository.GetByIdAsync(id);

        if (student is null)
            return Result.Failure("Öğrenci bulunamadı.", 404);

        await _studentRepository.DeleteAsync(id);

        return Result.Success("Öğrenci başarıyla silindi.");
    }
}