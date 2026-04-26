using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Students;
using EduPulse.Entities.Students;
using EduPulse.Repository.Abstracts;

namespace EduPulse.Business.Concretes;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly ISchoolRepository _schoolRepository;
    private readonly IClassroomRepository _classroomRepository;

    public StudentService(
        IStudentRepository studentRepository,
        ISchoolRepository schoolRepository,
        IClassroomRepository classroomRepository)
    {
        _studentRepository = studentRepository;
        _schoolRepository = schoolRepository;
        _classroomRepository = classroomRepository;
    }

    public async Task<List<StudentListDto>> GetAllAsync()
    {
        var students = await _studentRepository.GetAllAsync();
        var classrooms = await _classroomRepository.GetAllAsync();

        return students.Select(x =>
        {
            var classroom = classrooms.FirstOrDefault(c => c.Id == x.ClassroomId);

            return new StudentListDto
            {
                Id = x.Id,
                FullName = $"{x.FirstName} {x.LastName}",
                SchoolNumber = x.SchoolNumber,
                StudentPhone = x.StudentPhone,
                SchoolId = x.SchoolId,
                ClassroomId = x.ClassroomId,
                ClassroomName = classroom != null
                    ? $"{classroom.Grade}-{classroom.Section}"
                    : null,
                ClubIds = x.ClubIds,
                ParentIds = x.ParentIds,
                IsActive = x.IsActive
            };
        }).ToList();
    }

    public async Task<List<StudentListDto>> GetBySchoolIdAsync(string schoolId)
    {
        var students = await _studentRepository.GetBySchoolIdAsync(schoolId);
        var classrooms = await _classroomRepository.GetBySchoolIdAsync(schoolId);

        return students.Select(x =>
        {
            var classroom = classrooms.FirstOrDefault(c => c.Id == x.ClassroomId);

            return new StudentListDto
            {
                Id = x.Id,
                FullName = $"{x.FirstName} {x.LastName}",
                SchoolNumber = x.SchoolNumber,
                StudentPhone = x.StudentPhone,
                SchoolId = x.SchoolId,
                ClassroomId = x.ClassroomId,
                ClassroomName = classroom != null
                    ? $"{classroom.Grade}-{classroom.Section}"
                    : null,
                ClubIds = x.ClubIds,
                ParentIds = x.ParentIds,
                IsActive = x.IsActive
            };
        }).ToList();
    }

    public async Task<List<StudentListDto>> GetByClassroomIdAsync(string classroomId)
    {
        var students = await _studentRepository.GetByClassroomIdAsync(classroomId);
        var classroom = await _classroomRepository.GetByIdAsync(classroomId);

        return students.Select(x => new StudentListDto
        {
            Id = x.Id,
            FullName = $"{x.FirstName} {x.LastName}",
            SchoolNumber = x.SchoolNumber,
            StudentPhone = x.StudentPhone,
            SchoolId = x.SchoolId,
            ClassroomId = x.ClassroomId,
            ClassroomName = classroom != null
                ? $"{classroom.Grade}-{classroom.Section}"
                : null,
            ClubIds = x.ClubIds,
            ParentIds = x.ParentIds,
            IsActive = x.IsActive
        }).ToList();
    }

    public async Task<StudentListDto?> GetByIdAsync(string id)
    {
        var student = await _studentRepository.GetByIdAsync(id);

        if (student is null)
            return null;

        var classroom = await _classroomRepository.GetByIdAsync(student.ClassroomId);

        return new StudentListDto
        {
            Id = student.Id,
            FullName = $"{student.FirstName} {student.LastName}",
            SchoolNumber = student.SchoolNumber,
            StudentPhone = student.StudentPhone,
            SchoolId = student.SchoolId,
            ClassroomId = student.ClassroomId,
            ClassroomName = classroom != null
                ? $"{classroom.Grade}-{classroom.Section}"
                : null,
            ClubIds = student.ClubIds,
            ParentIds = student.ParentIds,
            IsActive = student.IsActive
        };
    }

    public async Task CreateAsync(CreateStudentDto dto)
    {
        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);
        if (school is null)
            throw new Exception("Okul bulunamadı.");

        var classroom = await _classroomRepository.GetByIdAsync(dto.ClassroomId);
        if (classroom is null)
            throw new Exception("Sınıf bulunamadı.");

        if (classroom.SchoolId != dto.SchoolId)
            throw new Exception("Seçilen sınıf bu okula ait değil.");

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
    }

    public async Task UpdateAsync(UpdateStudentDto dto)
    {
        var student = await _studentRepository.GetByIdAsync(dto.Id);
        if (student is null)
            throw new Exception("Öğrenci bulunamadı.");

        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);
        if (school is null)
            throw new Exception("Okul bulunamadı.");

        var classroom = await _classroomRepository.GetByIdAsync(dto.ClassroomId);
        if (classroom is null)
            throw new Exception("Sınıf bulunamadı.");

        if (classroom.SchoolId != dto.SchoolId)
            throw new Exception("Seçilen sınıf bu okula ait değil.");

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
    }

    public async Task DeleteAsync(string id)
    {
        var student = await _studentRepository.GetByIdAsync(id);

        if (student is null)
            throw new Exception("Öğrenci bulunamadı.");

        await _studentRepository.DeleteAsync(id);
    }
}