using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Common;
using EduPulse.DTOs.StudentGrades;
using EduPulse.Entities.StudentGrades;
using EduPulse.Repository.Abstracts;
using FluentValidation;

namespace EduPulse.Business.Concretes;

public class StudentGradeService : IStudentGradeService
{
    private readonly IStudentGradeRepository _studentGradeRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly ITeacherLessonRepository _teacherLessonRepository;
    private readonly IValidator<CreateStudentGradeDto> _createValidator;
    private readonly IValidator<UpdateStudentGradeDto> _updateValidator;

    public StudentGradeService(
        IStudentGradeRepository studentGradeRepository,
        IStudentRepository studentRepository,
        ILessonRepository lessonRepository,
        ITeacherRepository teacherRepository,
        ITeacherLessonRepository teacherLessonRepository,
        IValidator<CreateStudentGradeDto> createValidator,
        IValidator<UpdateStudentGradeDto> updateValidator)
    {
        _studentGradeRepository = studentGradeRepository;
        _studentRepository = studentRepository;
        _lessonRepository = lessonRepository;
        _teacherRepository = teacherRepository;
        _teacherLessonRepository = teacherLessonRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<List<StudentGradeListDto>>> GetAllAsync()
    {
        var grades = await _studentGradeRepository.GetAllAsync();

        var result = grades.Select(x => new StudentGradeListDto
        {
            Id = x.Id,
            StudentId = x.StudentId,
            LessonId = x.LessonId,
            Exam1 = x.Exam1,
            Exam2 = x.Exam2,
            Project = x.Project,
            Activity1 = x.Activity1,
            Activity2 = x.Activity2,
            Activity3 = x.Activity3,
            IsActive = x.IsActive
        }).ToList();

        return Result<List<StudentGradeListDto>>.Success(result, "Öğrenci notları başarıyla listelendi.");
    }

    public async Task<Result<List<StudentGradeListDto>>> GetByStudentIdAsync(string studentId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);

        if (student is null)
            return Result<List<StudentGradeListDto>>.Failure("Öğrenci bulunamadı.", 404);

        var grades = await _studentGradeRepository.GetByStudentIdAsync(studentId);

        var result = grades.Select(x => new StudentGradeListDto
        {
            Id = x.Id,
            StudentId = x.StudentId,
            LessonId = x.LessonId,
            Exam1 = x.Exam1,
            Exam2 = x.Exam2,
            Project = x.Project,
            Activity1 = x.Activity1,
            Activity2 = x.Activity2,
            Activity3 = x.Activity3,
            IsActive = x.IsActive
        }).ToList();

        return Result<List<StudentGradeListDto>>.Success(result, "Öğrenciye ait notlar başarıyla listelendi.");
    }

    public async Task<Result<List<StudentGradeListDto>>> GetByLessonIdAsync(string lessonId)
    {
        var lesson = await _lessonRepository.GetByIdAsync(lessonId);

        if (lesson is null)
            return Result<List<StudentGradeListDto>>.Failure("Ders bulunamadı.", 404);

        var grades = await _studentGradeRepository.GetByLessonIdAsync(lessonId);

        var result = grades.Select(x => new StudentGradeListDto
        {
            Id = x.Id,
            StudentId = x.StudentId,
            LessonId = x.LessonId,
            Exam1 = x.Exam1,
            Exam2 = x.Exam2,
            Project = x.Project,
            Activity1 = x.Activity1,
            Activity2 = x.Activity2,
            Activity3 = x.Activity3,
            IsActive = x.IsActive
        }).ToList();

        return Result<List<StudentGradeListDto>>.Success(result, "Derse ait notlar başarıyla listelendi.");
    }

    public async Task<Result<StudentGradeListDto>> GetByIdAsync(string id)
    {
        var grade = await _studentGradeRepository.GetByIdAsync(id);

        if (grade is null)
            return Result<StudentGradeListDto>.Failure("Not kaydı bulunamadı.", 404);

        var result = new StudentGradeListDto
        {
            Id = grade.Id,
            StudentId = grade.StudentId,
            LessonId = grade.LessonId,
            Exam1 = grade.Exam1,
            Exam2 = grade.Exam2,
            Project = grade.Project,
            Activity1 = grade.Activity1,
            Activity2 = grade.Activity2,
            Activity3 = grade.Activity3,
            IsActive = grade.IsActive
        };

        return Result<StudentGradeListDto>.Success(result, "Not kaydı başarıyla getirildi.");
    }

    public async Task<Result> CreateAsync(CreateStudentGradeDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var student = await _studentRepository.GetByIdAsync(dto.StudentId);

        if (student is null)
            return Result.Failure("Öğrenci bulunamadı.", 404);

        var lesson = await _lessonRepository.GetByIdAsync(dto.LessonId);

        if (lesson is null)
            return Result.Failure("Ders bulunamadı.", 404);

        if (student.SchoolId != dto.SchoolId)
            return Result.Failure("Öğrenci bu okula ait değil.", 400);

        var teacher = await _teacherRepository.GetByIdAsync(dto.TeacherId);

        if (teacher is null)
            return Result.Failure("Öğretmen bulunamadı.", 404);

        if (teacher.SchoolId != dto.SchoolId)
            return Result.Failure("Öğretmen bu okula ait değil.", 400);

        var teacherLesson = await _teacherLessonRepository.GetByTeacherLessonAndClassroomAsync(
            dto.TeacherId,
            dto.LessonId,
            student.ClassroomId
        );

        if (teacherLesson is null)
            return Result.Failure("Bu öğretmen bu öğrenciye bu dersten not giremez.", 403);

        var existingGrade = await _studentGradeRepository
            .GetByStudentAndLessonAsync(dto.StudentId, dto.LessonId);

        if (existingGrade is not null)
            return Result.Failure("Bu öğrenciye bu dersten zaten not girilmiş. Mevcut not kaydını güncelleyin.", 409);

        var average = (dto.Exam1 + dto.Exam2 + dto.Project + dto.Activity1 + dto.Activity2 + dto.Activity3) / 6.0;

        var grade = new StudentGrade
        {
            SchoolId = dto.SchoolId,
            TeacherId = dto.TeacherId,
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

        await _studentGradeRepository.CreateAsync(grade);

        return Result.Success("Öğrenci notu başarıyla oluşturuldu.", 201);
    }

    public async Task<Result> UpdateAsync(UpdateStudentGradeDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        var grade = await _studentGradeRepository.GetByIdAsync(dto.Id);

        if (grade is null)
            return Result.Failure("Not kaydı bulunamadı.", 404);

        var student = await _studentRepository.GetByIdAsync(dto.StudentId);

        if (student is null)
            return Result.Failure("Öğrenci bulunamadı.", 404);

        var lesson = await _lessonRepository.GetByIdAsync(dto.LessonId);

        if (lesson is null)
            return Result.Failure("Ders bulunamadı.", 404);

        if (student.SchoolId != dto.SchoolId)
            return Result.Failure("Öğrenci bu okula ait değil.", 400);

        var teacher = await _teacherRepository.GetByIdAsync(dto.TeacherId);

        if (teacher is null)
            return Result.Failure("Öğretmen bulunamadı.", 404);

        if (teacher.SchoolId != dto.SchoolId)
            return Result.Failure("Öğretmen bu okula ait değil.", 400);

        var teacherLesson = await _teacherLessonRepository.GetByTeacherLessonAndClassroomAsync(
            dto.TeacherId,
            dto.LessonId,
            student.ClassroomId
        );

        if (teacherLesson is null)
            return Result.Failure("Bu öğretmen bu öğrenciye bu dersten not giremez.", 403);

        var average = (dto.Exam1 + dto.Exam2 + dto.Project + dto.Activity1 + dto.Activity2 + dto.Activity3) / 6.0;

        grade.SchoolId = dto.SchoolId;
        grade.TeacherId = dto.TeacherId;
        grade.StudentId = dto.StudentId;
        grade.LessonId = dto.LessonId;
        grade.Exam1 = dto.Exam1;
        grade.Exam2 = dto.Exam2;
        grade.Project = dto.Project;
        grade.Activity1 = dto.Activity1;
        grade.Activity2 = dto.Activity2;
        grade.Activity3 = dto.Activity3;
        grade.Average = average;
        grade.IsActive = dto.IsActive;

        await _studentGradeRepository.UpdateAsync(grade);

        return Result.Success("Öğrenci notu başarıyla güncellendi.");
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var grade = await _studentGradeRepository.GetByIdAsync(id);

        if (grade is null)
            return Result.Failure("Not kaydı bulunamadı.", 404);

        await _studentGradeRepository.DeleteAsync(id);

        return Result.Success("Öğrenci notu başarıyla silindi.");
    }
}