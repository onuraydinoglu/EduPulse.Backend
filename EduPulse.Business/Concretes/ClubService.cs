using EduPulse.Business.Abstracts;
using EduPulse.DTOs.ClubMembers;
using EduPulse.DTOs.Clubs;
using EduPulse.DTOs.Common;
using EduPulse.Entities.ClubMembers;
using EduPulse.Entities.Clubs;
using EduPulse.Repository.Abstracts;
using FluentValidation;

namespace EduPulse.Business.Concretes;

public class ClubService : IClubService
{
    private readonly IClubRepository _clubRepository;
    private readonly IClubMemberRepository _clubMemberRepository;
    private readonly IUserRepository _userRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IValidator<CreateClubDto> _createClubValidator;
    private readonly IValidator<UpdateClubDto> _updateClubValidator;
    private readonly IValidator<AddClubMemberDto> _addClubMemberValidator;

    public ClubService(
        IClubRepository clubRepository,
        IClubMemberRepository clubMemberRepository,
        IUserRepository userRepository,
        IStudentRepository studentRepository,
        ITeacherRepository teacherRepository,
        IValidator<CreateClubDto> createClubValidator,
        IValidator<UpdateClubDto> updateClubValidator,
        IValidator<AddClubMemberDto> addClubMemberValidator)
    {
        _clubRepository = clubRepository;
        _clubMemberRepository = clubMemberRepository;
        _userRepository = userRepository;
        _studentRepository = studentRepository;
        _teacherRepository = teacherRepository;
        _createClubValidator = createClubValidator;
        _updateClubValidator = updateClubValidator;
        _addClubMemberValidator = addClubMemberValidator;
    }

    public async Task<Result<List<ClubListDto>>> GetAllForCurrentUserAsync(string? roleName, string? schoolId)
    {
        if (roleName != "schooladmin" && roleName != "teacher")
            return Result<List<ClubListDto>>.Failure("Kulüpleri listeleme yetkiniz yok.", 403);

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result<List<ClubListDto>>.Failure("Okul bilgisi bulunamadı.", 400);

        var clubs = await _clubRepository.GetBySchoolIdAsync(schoolId);
        var dtoList = new List<ClubListDto>();

        foreach (var club in clubs)
            dtoList.Add(await MapToListDtoAsync(club));

        return Result<List<ClubListDto>>.Success(dtoList, "Kulüpler başarıyla listelendi.", 200);
    }

    public async Task<Result<ClubListDto>> GetByIdForCurrentUserAsync(string id, string? roleName, string? schoolId)
    {
        if (roleName != "schooladmin" && roleName != "teacher")
            return Result<ClubListDto>.Failure("Kulüp görüntüleme yetkiniz yok.", 403);

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result<ClubListDto>.Failure("Okul bilgisi bulunamadı.", 400);

        var club = await _clubRepository.GetByIdAsync(id);

        if (club is null)
            return Result<ClubListDto>.Failure("Kulüp bulunamadı.", 404);

        if (club.SchoolId != schoolId)
            return Result<ClubListDto>.Failure("Bu kulübe erişim yetkiniz yok.", 403);

        return Result<ClubListDto>.Success(await MapToListDtoAsync(club), "Kulüp başarıyla getirildi.", 200);
    }

    public async Task<Result> CreateAsync(CreateClubDto dto, string? roleName, string? schoolId)
    {
        var validationResult = await _createClubValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        if (roleName != "schooladmin")
            return Result.Failure("Kulüp ekleme yetkiniz yok.", 403);

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        var normalizedName = NormalizeName(dto.Name);

        var existingClub = await _clubRepository.GetBySchoolIdAndNameAsync(schoolId, normalizedName);

        if (existingClub is not null)
            return Result.Failure("Bu okulda aynı isimde kulüp zaten mevcut.", 400);

        var teacher = await _teacherRepository.GetByIdAsync(dto.AdvisorTeacherId);

        if (teacher is null)
            return Result.Failure("Kulüp hocası bulunamadı.", 404);

        if (!teacher.IsActive)
            return Result.Failure("Pasif öğretmen kulüp hocası olarak seçilemez.", 400);

        if (teacher.SchoolId != schoolId)
            return Result.Failure("Seçilen öğretmen bu okula ait değil.", 400);

        var teacherUser = await _userRepository.GetByIdAsync(teacher.UserId);

        if (teacherUser is null)
            return Result.Failure("Öğretmene bağlı kullanıcı bulunamadı.", 404);

        var club = new Club
        {
            SchoolId = schoolId,
            Name = dto.Name.Trim(),
            NormalizedName = normalizedName,
            AdvisorTeacherId = dto.AdvisorTeacherId,
            IsActive = true
        };

        await _clubRepository.CreateAsync(club);

        return Result.Success("Kulüp başarıyla oluşturuldu.", 201);
    }

    public async Task<Result> UpdateAsync(UpdateClubDto dto, string? roleName, string? schoolId)
    {
        var validationResult = await _updateClubValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        if (roleName != "schooladmin")
            return Result.Failure("Kulüp güncelleme yetkiniz yok.", 403);

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        var club = await _clubRepository.GetByIdAsync(dto.Id);

        if (club is null)
            return Result.Failure("Kulüp bulunamadı.", 404);

        if (club.SchoolId != schoolId)
            return Result.Failure("Bu kulübü güncelleme yetkiniz yok.", 403);

        var normalizedName = NormalizeName(dto.Name);

        var existingClub = await _clubRepository.GetBySchoolIdAndNameAsync(club.SchoolId, normalizedName);

        if (existingClub is not null && existingClub.Id != dto.Id)
            return Result.Failure("Bu okulda aynı isimde kulüp zaten mevcut.", 400);

        var teacher = await _teacherRepository.GetByIdAsync(dto.AdvisorTeacherId);

        if (teacher is null)
            return Result.Failure("Kulüp hocası bulunamadı.", 404);

        if (!teacher.IsActive)
            return Result.Failure("Pasif öğretmen kulüp hocası olarak seçilemez.", 400);

        if (teacher.SchoolId != schoolId)
            return Result.Failure("Seçilen öğretmen bu okula ait değil.", 400);

        var teacherUser = await _userRepository.GetByIdAsync(teacher.UserId);

        if (teacherUser is null)
            return Result.Failure("Öğretmene bağlı kullanıcı bulunamadı.", 404);

        club.Name = dto.Name.Trim();
        club.NormalizedName = normalizedName;
        club.AdvisorTeacherId = dto.AdvisorTeacherId;
        club.IsActive = dto.IsActive;

        await _clubRepository.UpdateAsync(club);

        return Result.Success("Kulüp başarıyla güncellendi.", 200);
    }

    public async Task<Result> DeleteAsync(string id, string? roleName, string? schoolId)
    {
        if (roleName != "schooladmin")
            return Result.Failure("Kulüp silme yetkiniz yok.", 403);

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        var club = await _clubRepository.GetByIdAsync(id);

        if (club is null)
            return Result.Failure("Kulüp bulunamadı.", 404);

        if (club.SchoolId != schoolId)
            return Result.Failure("Bu kulübü silme yetkiniz yok.", 403);

        await _clubRepository.DeleteAsync(id);

        return Result.Success("Kulüp başarıyla silindi.", 200);
    }

    public async Task<Result> AddMemberAsync(AddClubMemberDto dto, string? roleName, string? schoolId)
    {
        var validationResult = await _addClubMemberValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        if (roleName != "schooladmin")
            return Result.Failure("Kulübe üye ekleme yetkiniz yok.", 403);

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

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

        var activeMembership = await _clubMemberRepository
            .GetActiveByClubIdAndStudentIdAsync(dto.ClubId, dto.StudentId);

        if (activeMembership is not null)
            return Result.Failure("Bu öğrenci zaten bu kulübe kayıtlı.", 400);

        var oldMembership = await _clubMemberRepository
            .GetAnyByClubIdAndStudentIdAsync(dto.ClubId, dto.StudentId);

        if (oldMembership is not null)
        {
            await _clubMemberRepository.ReactivateAsync(dto.ClubId, dto.StudentId);
            return Result.Success("Öğrenci kulübe tekrar aktif olarak eklendi.", 200);
        }

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

    public async Task<Result> RemoveMemberAsync(string clubId, string studentId, string? roleName, string? schoolId)
    {
        if (roleName != "schooladmin")
            return Result.Failure("Kulüpten üye çıkarma yetkiniz yok.", 403);

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        var club = await _clubRepository.GetByIdAsync(clubId);

        if (club is null)
            return Result.Failure("Kulüp bulunamadı.", 404);

        if (club.SchoolId != schoolId)
            return Result.Failure("Bu işlem için yetkiniz yok.", 403);

        var membership = await _clubMemberRepository
            .GetActiveByClubIdAndStudentIdAsync(clubId, studentId);

        if (membership is null)
            return Result.Failure("Öğrenci bu kulüpte kayıtlı değil.", 404);

        await _clubMemberRepository.DeleteAsync(clubId, studentId);

        return Result.Success("Öğrenci kulüpten başarıyla çıkarıldı.", 200);
    }

    public async Task<Result<List<ClubMemberListDto>>> GetMembersByClubIdAsync(string clubId, string? roleName, string? schoolId)
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

        var memberships = await _clubMemberRepository.GetByClubIdAsync(clubId);
        var dtoList = new List<ClubMemberListDto>();

        foreach (var membership in memberships)
        {
            var student = await _studentRepository.GetByIdAsync(membership.StudentId);
            var studentUser = student is not null
                ? await _userRepository.GetByIdAsync(student.UserId)
                : null;

            dtoList.Add(new ClubMemberListDto
            {
                Id = membership.Id,
                ClubId = membership.ClubId,
                StudentId = membership.StudentId,
                StudentFullName = studentUser is not null
                    ? $"{studentUser.FirstName} {studentUser.LastName}"
                    : null,
                StudentNumber = student?.StudentNumber,
                IsActive = membership.IsActive
            });
        }

        return Result<List<ClubMemberListDto>>.Success(dtoList, "Kulüp üyeleri başarıyla listelendi.", 200);
    }

    public async Task<Result<List<ClubListDto>>> GetClubsByStudentIdAsync(string studentId, string? roleName, string? schoolId)
    {
        if (roleName != "schooladmin" && roleName != "teacher")
            return Result<List<ClubListDto>>.Failure("Öğrencinin kulüplerini görüntüleme yetkiniz yok.", 403);

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result<List<ClubListDto>>.Failure("Okul bilgisi bulunamadı.", 400);

        var student = await _studentRepository.GetByIdAsync(studentId);

        if (student is null)
            return Result<List<ClubListDto>>.Failure("Öğrenci bulunamadı.", 404);

        if (student.SchoolId != schoolId)
            return Result<List<ClubListDto>>.Failure("Bu öğrencinin kulüplerini görüntüleme yetkiniz yok.", 403);

        var memberships = await _clubMemberRepository.GetByStudentIdAsync(studentId);
        var dtoList = new List<ClubListDto>();

        foreach (var membership in memberships)
        {
            var club = await _clubRepository.GetByIdAsync(membership.ClubId);

            if (club is not null)
                dtoList.Add(await MapToListDtoAsync(club));
        }

        return Result<List<ClubListDto>>.Success(dtoList, "Öğrencinin kulüpleri başarıyla listelendi.", 200);
    }

    private async Task<ClubListDto> MapToListDtoAsync(Club club)
    {
        var teacher = await _teacherRepository.GetByIdAsync(club.AdvisorTeacherId);

        string? advisorTeacherFullName = null;

        if (teacher is not null)
        {
            var teacherUser = await _userRepository.GetByIdAsync(teacher.UserId);

            if (teacherUser is not null)
                advisorTeacherFullName = $"{teacherUser.FirstName} {teacherUser.LastName}";
        }

        var memberCount = await _clubMemberRepository.GetActiveMemberCountByClubIdAsync(club.Id);

        return new ClubListDto
        {
            Id = club.Id,
            SchoolId = club.SchoolId,
            Name = club.Name,
            AdvisorTeacherId = club.AdvisorTeacherId,
            AdvisorTeacherFullName = advisorTeacherFullName,
            MemberCount = memberCount,
            IsActive = club.IsActive
        };
    }

    private static string NormalizeName(string name)
    {
        return name.Trim().ToLower();
    }
}