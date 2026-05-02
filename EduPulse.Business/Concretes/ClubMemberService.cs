using EduPulse.Business.Abstracts;
using EduPulse.DTOs.ClubMembers;
using EduPulse.DTOs.Common;
using EduPulse.Entities.ClubMembers;
using EduPulse.Repository.Abstracts;

namespace EduPulse.Business.Concretes;

public class ClubMemberService : IClubMemberService
{
    private readonly IClubMemberRepository _clubMemberRepository;
    private readonly IClubRepository _clubRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IClassroomRepository _classroomRepository;

    public ClubMemberService(
        IClubMemberRepository clubMemberRepository,
        IClubRepository clubRepository,
        IStudentRepository studentRepository,
        IUserRepository userRepository,
        IClassroomRepository classroomRepository)
    {
        _clubMemberRepository = clubMemberRepository;
        _clubRepository = clubRepository;
        _studentRepository = studentRepository;
        _userRepository = userRepository;
        _classroomRepository = classroomRepository;
    }

    public async Task<Result<List<ClubMemberListDto>>> GetAllForCurrentUserAsync(
        string? roleName,
        string? schoolId)
    {
        if (roleName != "schooladmin" && roleName != "teacher")
            return Result<List<ClubMemberListDto>>.Failure("Kulüp üyelerini listeleme yetkiniz yok.", 403);

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result<List<ClubMemberListDto>>.Failure("Okul bilgisi bulunamadı.", 400);

        var members = await _clubMemberRepository.GetBySchoolIdAsync(schoolId);
        var list = await MapToListDtoAsync(members);

        return Result<List<ClubMemberListDto>>.Success(list, "Kulüp üyeleri başarıyla listelendi.", 200);
    }

    public async Task<Result<List<ClubMemberListDto>>> GetByClubIdForCurrentUserAsync(
        string clubId,
        string? roleName,
        string? schoolId)
    {
        if (roleName != "schooladmin" && roleName != "teacher")
            return Result<List<ClubMemberListDto>>.Failure("Kulüp üyelerini görüntüleme yetkiniz yok.", 403);

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result<List<ClubMemberListDto>>.Failure("Okul bilgisi bulunamadı.", 400);

        var club = await _clubRepository.GetByIdAsync(clubId);

        if (club is null)
            return Result<List<ClubMemberListDto>>.Failure("Kulüp bulunamadı.", 404);

        if (club.SchoolId != schoolId)
            return Result<List<ClubMemberListDto>>.Failure("Bu kulübün üyelerini görüntüleme yetkiniz yok.", 403);

        var members = await _clubMemberRepository.GetByClubIdAsync(clubId);
        var list = await MapToListDtoAsync(members);

        return Result<List<ClubMemberListDto>>.Success(list, "Kulüp üyeleri başarıyla listelendi.", 200);
    }

    public async Task<Result<List<ClubMemberListDto>>> GetByStudentIdForCurrentUserAsync(
        string studentId,
        string? roleName,
        string? schoolId)
    {
        if (roleName != "schooladmin" && roleName != "teacher")
            return Result<List<ClubMemberListDto>>.Failure("Öğrencinin kulüp üyeliklerini görüntüleme yetkiniz yok.", 403);

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result<List<ClubMemberListDto>>.Failure("Okul bilgisi bulunamadı.", 400);

        var student = await _studentRepository.GetByIdAsync(studentId);

        if (student is null)
            return Result<List<ClubMemberListDto>>.Failure("Öğrenci bulunamadı.", 404);

        if (student.SchoolId != schoolId)
            return Result<List<ClubMemberListDto>>.Failure("Bu öğrencinin üyeliklerini görüntüleme yetkiniz yok.", 403);

        var members = await _clubMemberRepository.GetByStudentIdAsync(studentId);
        var list = await MapToListDtoAsync(members);

        return Result<List<ClubMemberListDto>>.Success(list, "Öğrencinin kulüp üyelikleri başarıyla listelendi.", 200);
    }

    public async Task<Result> CreateAsync(CreateClubMemberDto dto, string? roleName, string? schoolId)
    {
        if (roleName != "schooladmin")
            return Result.Failure("Kulübe üye ekleme yetkiniz yok.", 403);

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        if (string.IsNullOrWhiteSpace(dto.ClubId))
            return Result.Failure("Kulüp seçiniz.", 400);

        if (string.IsNullOrWhiteSpace(dto.StudentId))
            return Result.Failure("Öğrenci seçiniz.", 400);

        var club = await _clubRepository.GetByIdAsync(dto.ClubId);

        if (club is null)
            return Result.Failure("Kulüp bulunamadı.", 404);

        if (club.SchoolId != schoolId)
            return Result.Failure("Bu kulübe üye ekleme yetkiniz yok.", 403);

        if (!club.IsActive)
            return Result.Failure("Pasif kulübe üye eklenemez.", 400);

        var student = await _studentRepository.GetByIdAsync(dto.StudentId);

        if (student is null)
            return Result.Failure("Öğrenci bulunamadı.", 404);

        if (student.SchoolId != schoolId)
            return Result.Failure("Seçilen öğrenci bu okula ait değil.", 400);

        if (!student.IsActive)
            return Result.Failure("Pasif öğrenci kulübe eklenemez.", 400);

        var existingMember = await _clubMemberRepository.GetByClubIdAndStudentIdAsync(
            dto.ClubId,
            dto.StudentId
        );

        if (existingMember is not null)
            return Result.Failure("Bu öğrenci zaten bu kulübe kayıtlı.", 400);

        var clubMember = new ClubMember
        {
            SchoolId = schoolId,
            ClubId = dto.ClubId,
            StudentId = dto.StudentId,
            IsActive = true
        };

        await _clubMemberRepository.CreateAsync(clubMember);

        return Result.Success("Öğrenci kulübe başarıyla eklendi.", 201);
    }

    public async Task<Result> DeleteAsync(string id, string? roleName, string? schoolId)
    {
        if (roleName != "schooladmin")
            return Result.Failure("Kulüpten üye çıkarma yetkiniz yok.", 403);

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        var member = await _clubMemberRepository.GetByIdAsync(id);

        if (member is null || !member.IsActive)
            return Result.Failure("Kulüp üyesi bulunamadı.", 404);

        if (member.SchoolId != schoolId)
            return Result.Failure("Bu üyeliği silme yetkiniz yok.", 403);

        await _clubMemberRepository.DeleteAsync(id);

        return Result.Success("Öğrenci kulüpten başarıyla çıkarıldı.", 200);
    }

    private async Task<List<ClubMemberListDto>> MapToListDtoAsync(List<ClubMember> members)
    {
        var clubs = await _clubRepository.GetAllAsync();
        var students = await _studentRepository.GetAllAsync();
        var users = await _userRepository.GetAllAsync();
        var classrooms = await _classroomRepository.GetAllAsync();

        return members.Select(member =>
        {
            var club = clubs.FirstOrDefault(x => x.Id == member.ClubId);
            var student = students.FirstOrDefault(x => x.Id == member.StudentId);

            var user = student is null
                ? null
                : users.FirstOrDefault(x => x.Id == student.UserId);

            var classroom = student is null
                ? null
                : classrooms.FirstOrDefault(x => x.Id == student.ClassroomId);

            var studentFullName = user is null
                ? "-"
                : $"{user.FirstName} {user.LastName}";

            var classroomName = classroom is null
                ? "-"
                : $"{classroom.Grade}. Sınıf {classroom.Section}";

            return new ClubMemberListDto
            {
                Id = member.Id,

                ClubId = member.ClubId,
                ClubName = club?.Name ?? "-",

                StudentId = member.StudentId,
                StudentFullName = studentFullName,
                StudentNumber = student?.StudentNumber ?? "-",

                ClassroomId = student?.ClassroomId ?? "",
                ClassroomName = classroomName,

                IsActive = member.IsActive
            };
        }).ToList();
    }
}