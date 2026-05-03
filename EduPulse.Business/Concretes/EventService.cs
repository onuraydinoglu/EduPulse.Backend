using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Common;
using EduPulse.DTOs.Events;
using EduPulse.Entities.Events;
using EduPulse.Repository.Abstracts;
using FluentValidation;

namespace EduPulse.Business.Concretes;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IUserRepository _userRepository;
    private readonly IValidator<CreateEventDto> _createEventValidator;
    private readonly IValidator<UpdateEventDto> _updateEventValidator;

    public EventService(
        IEventRepository eventRepository,
        ITeacherRepository teacherRepository,
        IUserRepository userRepository,
        IValidator<CreateEventDto> createEventValidator,
        IValidator<UpdateEventDto> updateEventValidator)
    {
        _eventRepository = eventRepository;
        _teacherRepository = teacherRepository;
        _userRepository = userRepository;
        _createEventValidator = createEventValidator;
        _updateEventValidator = updateEventValidator;
    }

    public async Task<Result<List<EventListDto>>> GetAllForCurrentUserAsync(string? roleName, string? schoolId)
    {
        if (roleName != "schooladmin" && roleName != "officer")
            return Result<List<EventListDto>>.Failure("Etkinlikleri listeleme yetkiniz yok.", 403);

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result<List<EventListDto>>.Failure("Okul bilgisi bulunamadı.", 400);

        var events = await _eventRepository.GetBySchoolIdAsync(schoolId);

        var dtoList = new List<EventListDto>();

        foreach (var eventEntity in events)
        {
            dtoList.Add(await MapToListDtoAsync(eventEntity));
        }

        return Result<List<EventListDto>>.Success(dtoList, "Etkinlikler başarıyla listelendi.", 200);
    }

    public async Task<Result<EventListDto>> GetByIdForCurrentUserAsync(string id, string? roleName, string? schoolId)
    {
        if (roleName != "schooladmin" && roleName != "officer")
            return Result<EventListDto>.Failure("Etkinlik görüntüleme yetkiniz yok.", 403);

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result<EventListDto>.Failure("Okul bilgisi bulunamadı.", 400);

        var eventEntity = await _eventRepository.GetByIdAsync(id);

        if (eventEntity is null)
            return Result<EventListDto>.Failure("Etkinlik bulunamadı.", 404);

        if (eventEntity.SchoolId != schoolId)
            return Result<EventListDto>.Failure("Bu etkinliğe erişim yetkiniz yok.", 403);

        return Result<EventListDto>.Success(await MapToListDtoAsync(eventEntity), "Etkinlik başarıyla getirildi.", 200);
    }

    public async Task<Result> CreateAsync(CreateEventDto dto, string? roleName, string? schoolId)
    {
        var validationResult = await _createEventValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        if (roleName != "schooladmin" && roleName != "officer")
            return Result.Failure("Etkinlik ekleme yetkiniz yok.", 403);

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        var normalizedName = NormalizeName(dto.Name);

        var existingEvent = await _eventRepository.GetBySchoolIdAndNameAsync(schoolId, normalizedName);

        if (existingEvent is not null)
            return Result.Failure("Bu okulda aynı isimde aktif bir etkinlik zaten mevcut.", 400);

        var teacherValidationResult = await ValidateResponsibleTeachersAsync(dto.ResponsibleTeacherIds, schoolId);

        if (!teacherValidationResult.IsSuccess)
            return teacherValidationResult;

        var eventEntity = new Event
        {
            SchoolId = schoolId,
            Name = dto.Name.Trim(),
            NormalizedName = normalizedName,
            Description = dto.Description?.Trim(),
            Location = dto.Location.Trim(),
            EventDate = dto.EventDate,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            IsPaid = dto.IsPaid,
            PricePerStudent = dto.IsPaid ? dto.PricePerStudent : 0,
            Quota = dto.Quota,
            ResponsibleTeacherIds = dto.ResponsibleTeacherIds.Distinct().ToList(),
            IsActive = true
        };

        await _eventRepository.CreateAsync(eventEntity);

        return Result.Success("Etkinlik başarıyla oluşturuldu.", 201);
    }

    public async Task<Result> UpdateAsync(UpdateEventDto dto, string? roleName, string? schoolId)
    {
        var validationResult = await _updateEventValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors.First().ErrorMessage, 400);

        if (roleName != "schooladmin" && roleName != "officer")
            return Result.Failure("Etkinlik güncelleme yetkiniz yok.", 403);

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        var eventEntity = await _eventRepository.GetByIdAsync(dto.Id);

        if (eventEntity is null)
            return Result.Failure("Etkinlik bulunamadı.", 404);

        if (eventEntity.SchoolId != schoolId)
            return Result.Failure("Bu etkinliği güncelleme yetkiniz yok.", 403);

        var normalizedName = NormalizeName(dto.Name);

        var existingEvent = await _eventRepository.GetBySchoolIdAndNameAsync(schoolId, normalizedName);

        if (existingEvent is not null && existingEvent.Id != dto.Id)
            return Result.Failure("Bu okulda aynı isimde aktif bir etkinlik zaten mevcut.", 400);

        var teacherValidationResult = await ValidateResponsibleTeachersAsync(dto.ResponsibleTeacherIds, schoolId);

        if (!teacherValidationResult.IsSuccess)
            return teacherValidationResult;

        eventEntity.Name = dto.Name.Trim();
        eventEntity.NormalizedName = normalizedName;
        eventEntity.Description = dto.Description?.Trim();
        eventEntity.Location = dto.Location.Trim();
        eventEntity.EventDate = dto.EventDate;
        eventEntity.StartTime = dto.StartTime;
        eventEntity.EndTime = dto.EndTime;
        eventEntity.IsPaid = dto.IsPaid;
        eventEntity.PricePerStudent = dto.IsPaid ? dto.PricePerStudent : 0;
        eventEntity.Quota = dto.Quota;
        eventEntity.ResponsibleTeacherIds = dto.ResponsibleTeacherIds.Distinct().ToList();
        eventEntity.IsActive = dto.IsActive;

        await _eventRepository.UpdateAsync(eventEntity);

        return Result.Success("Etkinlik başarıyla güncellendi.", 200);
    }

    public async Task<Result> DeleteAsync(string id, string? roleName, string? schoolId)
    {
        if (roleName != "schooladmin" && roleName != "officer")
            return Result.Failure("Etkinlik silme yetkiniz yok.", 403);

        if (string.IsNullOrWhiteSpace(schoolId))
            return Result.Failure("Okul bilgisi bulunamadı.", 400);

        var eventEntity = await _eventRepository.GetByIdAsync(id);

        if (eventEntity is null)
            return Result.Failure("Etkinlik bulunamadı.", 404);

        if (eventEntity.SchoolId != schoolId)
            return Result.Failure("Bu etkinliği silme yetkiniz yok.", 403);

        await _eventRepository.DeleteAsync(id);

        return Result.Success("Etkinlik başarıyla silindi.", 200);
    }

    private async Task<Result> ValidateResponsibleTeachersAsync(List<string> teacherIds, string schoolId)
    {
        var uniqueTeacherIds = teacherIds.Distinct().ToList();

        foreach (var teacherId in uniqueTeacherIds)
        {
            var teacher = await _teacherRepository.GetByIdAsync(teacherId);

            if (teacher is null)
                return Result.Failure("Sorumlu öğretmen bulunamadı.", 404);

            if (!teacher.IsActive)
                return Result.Failure("Pasif öğretmen sorumlu öğretmen olarak seçilemez.", 400);

            if (teacher.SchoolId != schoolId)
                return Result.Failure("Seçilen sorumlu öğretmen bu okula ait değil.", 400);
        }

        return Result.Success("Öğretmen kontrolü başarılı.", 200);
    }

    private async Task<EventListDto> MapToListDtoAsync(Event eventEntity)
    {
        var responsibleTeachers = new List<EventTeacherDto>();

        foreach (var teacherId in eventEntity.ResponsibleTeacherIds)
        {
            var teacher = await _teacherRepository.GetByIdAsync(teacherId);

            if (teacher is null)
                continue;

            var user = await _userRepository.GetByIdAsync(teacher.UserId);

            responsibleTeachers.Add(new EventTeacherDto
            {
                TeacherId = teacher.Id,
                UserId = teacher.UserId,
                FullName = user is not null
                    ? $"{user.FirstName} {user.LastName}"
                    : null
            });
        }

        return new EventListDto
        {
            Id = eventEntity.Id,
            SchoolId = eventEntity.SchoolId,
            Name = eventEntity.Name,
            Description = eventEntity.Description,
            Location = eventEntity.Location,
            EventDate = eventEntity.EventDate,
            StartTime = eventEntity.StartTime,
            EndTime = eventEntity.EndTime,
            IsPaid = eventEntity.IsPaid,
            PricePerStudent = eventEntity.PricePerStudent,
            Quota = eventEntity.Quota,
            ResponsibleTeachers = responsibleTeachers,
            IsActive = eventEntity.IsActive
        };
    }

    private static string NormalizeName(string name)
    {
        return name.Trim().ToLower();
    }
}