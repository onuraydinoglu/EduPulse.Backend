using EduPulse.Business.Abstracts;
using EduPulse.DTOs.StudentGrades;
using EduPulse.Entities.StudentGrades;
using EduPulse.Repository.Abstracts;

namespace EduPulse.Business.Concretes;

public class StudentGradeService : IStudentGradeService
{
    private readonly IStudentGradeRepository _repository;
    private readonly IStudentRepository _studentRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly ISchoolRepository _schoolRepository;

    public StudentGradeService(
        IStudentGradeRepository repository,
        IStudentRepository studentRepository,
        ILessonRepository lessonRepository,
        ISchoolRepository schoolRepository)
    {
        _repository = repository;
        _studentRepository = studentRepository;
        _lessonRepository = lessonRepository;
        _schoolRepository = schoolRepository;
    }

    public async Task<List<StudentGradeListDto>> GetAllAsync()
    {
        var list = await _repository.GetAllAsync();
        var students = await _studentRepository.GetAllAsync();
        var lessons = await _lessonRepository.GetAllAsync();

        return list.Select(x => new StudentGradeListDto
        {
            Id = x.Id,
            SchoolId = x.SchoolId,
            StudentId = x.StudentId,
            StudentName = students.FirstOrDefault(s => s.Id == x.StudentId) != null
                ? $"{students.First(s => s.Id == x.StudentId).FirstName} {students.First(s => s.Id == x.StudentId).LastName}"
                : "",
            LessonId = x.LessonId,
            LessonName = lessons.FirstOrDefault(l => l.Id == x.LessonId)?.Name ?? "",
            Exam1 = x.Exam1,
            Exam2 = x.Exam2,
            Project = x.Project,
            Activity1 = x.Activity1,
            Activity2 = x.Activity2,
            Activity3 = x.Activity3,
            Average = x.Average,
            IsActive = x.IsActive
        }).ToList();
    }

    public async Task<List<StudentGradeListDto>> GetByStudentIdAsync(string studentId)
    {
        var list = await _repository.GetByStudentIdAsync(studentId);
        var lessons = await _lessonRepository.GetAllAsync();
        var student = await _studentRepository.GetByIdAsync(studentId);

        return list.Select(x => new StudentGradeListDto
        {
            Id = x.Id,
            SchoolId = x.SchoolId,
            StudentId = x.StudentId,
            StudentName = student != null ? $"{student.FirstName} {student.LastName}" : "",
            LessonId = x.LessonId,
            LessonName = lessons.FirstOrDefault(l => l.Id == x.LessonId)?.Name ?? "",
            Exam1 = x.Exam1,
            Exam2 = x.Exam2,
            Project = x.Project,
            Activity1 = x.Activity1,
            Activity2 = x.Activity2,
            Activity3 = x.Activity3,
            Average = x.Average,
            IsActive = x.IsActive
        }).ToList();
    }

    public async Task<List<StudentGradeListDto>> GetByLessonIdAsync(string lessonId)
    {
        var list = await _repository.GetByLessonIdAsync(lessonId);
        var students = await _studentRepository.GetAllAsync();
        var lesson = await _lessonRepository.GetByIdAsync(lessonId);

        return list.Select(x => new StudentGradeListDto
        {
            Id = x.Id,
            SchoolId = x.SchoolId,
            StudentId = x.StudentId,
            StudentName = students.FirstOrDefault(s => s.Id == x.StudentId) != null
                ? $"{students.First(s => s.Id == x.StudentId).FirstName} {students.First(s => s.Id == x.StudentId).LastName}"
                : "",
            LessonId = x.LessonId,
            LessonName = lesson?.Name ?? "",
            Exam1 = x.Exam1,
            Exam2 = x.Exam2,
            Project = x.Project,
            Activity1 = x.Activity1,
            Activity2 = x.Activity2,
            Activity3 = x.Activity3,
            Average = x.Average,
            IsActive = x.IsActive
        }).ToList();
    }

    public async Task CreateAsync(CreateStudentGradeDto dto)
    {
        var school = await _schoolRepository.GetByIdAsync(dto.SchoolId);
        if (school is null) throw new Exception("Okul bulunamadı.");

        var student = await _studentRepository.GetByIdAsync(dto.StudentId);
        if (student is null) throw new Exception("Öğrenci bulunamadı.");

        var lesson = await _lessonRepository.GetByIdAsync(dto.LessonId);
        if (lesson is null) throw new Exception("Ders bulunamadı.");

        var existing = await _repository.GetByStudentAndLessonAsync(dto.StudentId, dto.LessonId);
        if (existing != null)
            throw new Exception("Bu öğrenci için bu derste zaten not girilmiş.");

        var average = CalculateAverage(dto);

        var entity = new StudentGrade
        {
            SchoolId = dto.SchoolId,
            StudentId = dto.StudentId,
            LessonId = dto.LessonId,
            Exam1 = dto.Exam1,
            Exam2 = dto.Exam2,
            Project = dto.Project,
            Activity1 = dto.Activity1,
            Activity2 = dto.Activity2,
            Activity3 = dto.Activity3,
            Average = average,
            IsActive = true
        };

        await _repository.CreateAsync(entity);
    }

    public async Task UpdateAsync(UpdateStudentGradeDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id);
        if (entity is null) throw new Exception("Not bulunamadı.");

        var average = CalculateAverage(dto);

        entity.Exam1 = dto.Exam1;
        entity.Exam2 = dto.Exam2;
        entity.Project = dto.Project;
        entity.Activity1 = dto.Activity1;
        entity.Activity2 = dto.Activity2;
        entity.Activity3 = dto.Activity3;
        entity.Average = average;
        entity.IsActive = dto.IsActive;

        await _repository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(string id)
    {
        await _repository.DeleteAsync(id);
    }

    // 🔥 EN ÖNEMLİ KISIM
    private double CalculateAverage(dynamic dto)
    {
        var total =
            dto.Exam1 +
            dto.Exam2 +
            dto.Project +
            dto.Activity1 +
            dto.Activity2 +
            dto.Activity3;

        return total / 6.0;
    }
}