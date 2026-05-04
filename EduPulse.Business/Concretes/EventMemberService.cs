using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Common;
using EduPulse.DTOs.EventMembers;
using EduPulse.Entities.EventMembers;
using EduPulse.Repository.Abstracts;

namespace EduPulse.Business.Concretes;

public class EventMemberService : IEventMemberService
{
    private readonly IEventMemberRepository _eventMemberRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IClassroomRepository _classroomRepository;

    public EventMemberService(
        IEventMemberRepository eventMemberRepository,
        IEventRepository eventRepository,
        IStudentRepository studentRepository,
        IUserRepository userRepository,
        IClassroomRepository classroomRepository)
    {
        _eventMemberRepository = eventMemberRepository;
        _eventRepository = eventRepository;
        _studentRepository = studentRepository;
        _userRepository = userRepository;
        _classroomRepository = classroomRepository;
    }

    public async Task<Result<List<EventMemberListDto>>> GetAllForCurrentUserAsync(string? roleName, string? schoolId)
    {
        if (roleName != "superadmin" && string.IsNullOrWhiteSpace(schoolId))
            return Result<List<EventMemberListDto>>.Failure("Okul bilgisi bulunamadı.", 400);

        var members = roleName == "superadmin"
            ? await _eventMemberRepository.GetAllAsync()
            : await _eventMemberRepository.GetBySchoolIdAsync(schoolId!);

        var dtoList = await MapToDtoListAsync(members);

        return Result<List<EventMemberListDto>>.Success(dtoList, "Etkinlik üyeleri listelendi.");
    }

    public async Task<Result<List<EventMemberListDto>>> GetByEventIdForCurrentUserAsync(string eventId, string? roleName, string? schoolId)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(eventId);

        if (eventEntity is null || !eventEntity.IsActive)
            return Result<List<EventMemberListDto>>.Failure("Etkinlik bulunamadı.", 404);

        if (roleName != "superadmin" && eventEntity.SchoolId != schoolId)
            return Result<List<EventMemberListDto>>.Failure("Bu etkinliğe erişim yetkiniz yok.", 403);

        var members = await _eventMemberRepository.GetByEventIdAsync(eventId);
        var dtoList = await MapToDtoListAsync(members);

        return Result<List<EventMemberListDto>>.Success(dtoList, "Etkinlik üyeleri listelendi.");
    }

    public async Task<Result<List<EventMemberListDto>>> GetByStudentIdForCurrentUserAsync(string studentId, string? roleName, string? schoolId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);

        if (student is null || !student.IsActive)
            return Result<List<EventMemberListDto>>.Failure("Öğrenci bulunamadı.", 404);

        if (roleName != "superadmin" && student.SchoolId != schoolId)
            return Result<List<EventMemberListDto>>.Failure("Bu öğrenciye erişim yetkiniz yok.", 403);

        var members = await _eventMemberRepository.GetByStudentIdAsync(studentId);
        var dtoList = await MapToDtoListAsync(members);

        return Result<List<EventMemberListDto>>.Success(dtoList, "Öğrencinin etkinlik kayıtları listelendi.");
    }

    public async Task<Result> CreateAsync(CreateEventMemberDto dto, string? roleName, string? schoolId)
    {
        if (roleName != "schooladmin" && roleName != "officer")
            return Result.Failure("Etkinliğe öğrenci kaydetme yetkiniz yok.", 403);

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        var eventEntity = await _eventRepository.GetByIdAsync(dto.EventId);

        if (eventEntity is null || !eventEntity.IsActive)
            return Result.Failure("Etkinlik bulunamadı.", 404);

        if (eventEntity.SchoolId != schoolId)
            return Result.Failure("Bu etkinliğe öğrenci kaydedemezsiniz.", 403);

        var student = await _studentRepository.GetByIdAsync(dto.StudentId);

        if (student is null || !student.IsActive)
            return Result.Failure("Öğrenci bulunamadı.", 404);

        if (student.SchoolId != schoolId)
            return Result.Failure("Bu öğrenci sizin okulunuza ait değil.", 403);

        var duplicate = await _eventMemberRepository.GetByEventIdAndStudentIdAsync(dto.EventId, dto.StudentId);

        if (duplicate is not null)
            return Result.Failure("Bu öğrenci zaten etkinliğe kayıtlı.", 409);

        if (dto.PaidAmount < 0)
            return Result.Failure("Ödenen tutar 0'dan küçük olamaz.", 400);

        var eventMember = new EventMember
        {
            SchoolId = schoolId,
            EventId = dto.EventId,
            StudentId = dto.StudentId,
            IsPaid = dto.IsPaid,
            PaidAmount = dto.PaidAmount,
            IsActive = true
        };

        await _eventMemberRepository.CreateAsync(eventMember);

        return Result.Success("Öğrenci etkinliğe başarıyla kaydedildi.", 201);
    }

    public async Task<Result> UpdatePaymentAsync(UpdateEventMemberPaymentDto dto, string? roleName, string? schoolId)
    {
        if (roleName != "schooladmin" && roleName != "officer")
            return Result.Failure("Ödeme bilgisi güncelleme yetkiniz yok.", 403);

        var member = await _eventMemberRepository.GetByIdAsync(dto.Id);

        if (member is null || !member.IsActive)
            return Result.Failure("Etkinlik üyesi bulunamadı.", 404);

        if (roleName != "superadmin" && member.SchoolId != schoolId)
            return Result.Failure("Bu kaydı güncelleme yetkiniz yok.", 403);

        if (dto.PaidAmount < 0)
            return Result.Failure("Ödenen tutar 0'dan küçük olamaz.", 400);

        member.IsPaid = dto.IsPaid;
        member.PaidAmount = dto.PaidAmount;

        await _eventMemberRepository.UpdatePaymentAsync(member);

        return Result.Success("Ödeme bilgisi güncellendi.");
    }

    public async Task<Result> DeleteAsync(string id, string? roleName, string? schoolId)
    {
        if (roleName != "schooladmin" && roleName != "officer")
            return Result.Failure("Etkinlik kaydı silme yetkiniz yok.", 403);

        var member = await _eventMemberRepository.GetByIdAsync(id);

        if (member is null || !member.IsActive)
            return Result.Failure("Etkinlik üyesi bulunamadı.", 404);

        if (roleName != "superadmin" && member.SchoolId != schoolId)
            return Result.Failure("Bu kaydı silme yetkiniz yok.", 403);

        await _eventMemberRepository.DeleteAsync(id);

        return Result.Success("Öğrenci etkinlikten çıkarıldı.");
    }

    private async Task<List<EventMemberListDto>> MapToDtoListAsync(List<EventMember> members)
    {
        var dtoList = new List<EventMemberListDto>();

        foreach (var member in members)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(member.EventId);
            var student = await _studentRepository.GetByIdAsync(member.StudentId);

            var user = student is not null
                ? await _userRepository.GetByIdAsync(student.UserId)
                : null;

            var classroom = student is not null && !string.IsNullOrWhiteSpace(student.ClassroomId)
                ? await _classroomRepository.GetByIdAsync(student.ClassroomId)
                : null;

            dtoList.Add(new EventMemberListDto
            {
                Id = member.Id,

                EventId = member.EventId,
                EventName = eventEntity?.Name ?? "-",

                StudentId = member.StudentId,
                StudentFullName = user is not null ? $"{user.FirstName} {user.LastName}" : "-",
                StudentNumber = student?.StudentNumber ?? "-",

                ClassroomId = student?.ClassroomId ?? "",
                ClassroomName = classroom is not null
                    ? $"{classroom.Grade}/{classroom.Section}"
                    : "-",

                IsPaid = member.IsPaid,
                PaidAmount = member.PaidAmount,

                IsActive = member.IsActive
            });
        }

        return dtoList;
    }
}