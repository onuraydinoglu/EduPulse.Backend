using EduPulse.Business.Abstracts;
using EduPulse.DTOs.Common;
using EduPulse.DTOs.Messages;
using EduPulse.Entities.Classrooms;
using EduPulse.Entities.Messages;
using EduPulse.Entities.Users;
using EduPulse.Repository.Abstracts;

namespace EduPulse.Business.Concretes;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IClassroomRepository _classroomRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly ITeacherLessonRepository _teacherLessonRepository;

    private static readonly string[] AllowedMessageRoles =
    {
    "schooladmin",
    "teacher",
    "officer"
};

    private const string ClassroomTargetPrefix = "classroom:";

    public MessageService(
     IMessageRepository messageRepository,
     IUserRepository userRepository,
     IClassroomRepository classroomRepository,
     IStudentRepository studentRepository,
     ITeacherRepository teacherRepository,
     ITeacherLessonRepository teacherLessonRepository
 )
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _classroomRepository = classroomRepository;
        _studentRepository = studentRepository;
        _teacherRepository = teacherRepository;
        _teacherLessonRepository = teacherLessonRepository;
    }

    public async Task<Result<List<MessageUserListDto>>> GetMessageUsersAsync(
    string? currentUserId,
    string? currentRoleName,
    string? currentSchoolId
)
    {
        if (string.IsNullOrWhiteSpace(currentUserId))
        {
            return Result<List<MessageUserListDto>>.Failure(
                "Kullanıcı bilgisi bulunamadı.",
                400
            );
        }

        if (string.IsNullOrWhiteSpace(currentSchoolId))
        {
            return Result<List<MessageUserListDto>>.Failure(
                "Okul bilgisi bulunamadı.",
                400
            );
        }

        if (!IsAllowedRole(currentRoleName))
        {
            return Result<List<MessageUserListDto>>.Failure(
                "Mesaj kullanıcılarını listeleme yetkiniz yok.",
                403
            );
        }

        var users = await _userRepository.GetBySchoolIdAsync(currentSchoolId);

        var result = users
            .Where(x =>
                x.Id != currentUserId &&
                x.IsActive &&
                AllowedMessageRoles.Contains(x.RoleName.ToLower())
            )
            .OrderBy(x => GetRoleOrder(x.RoleName))
            .ThenBy(x => x.FirstName)
            .ThenBy(x => x.LastName)
            .Select(x => new MessageUserListDto
            {
                UserId = x.Id,
                FullName = $"{x.FirstName} {x.LastName}",
                RoleName = x.RoleName,
                Email = x.Email
            })
            .ToList();

        var classroomTargets = await GetClassroomMessageTargetsAsync(
            currentUserId,
            currentRoleName!,
            currentSchoolId
        );

        result.AddRange(classroomTargets);

        return Result<List<MessageUserListDto>>.Success(
            result,
            "Mesaj kullanıcıları başarıyla listelendi.",
            200
        );
    }

    public async Task<Result<List<MessageListDto>>> GetInboxAsync(
        string? currentUserId,
        string? currentSchoolId
    )
    {
        var validation = ValidateUserAndSchool(currentUserId, currentSchoolId);
        if (validation is not null)
        {
            return Result<List<MessageListDto>>.Failure(
                validation.Message,
                validation.StatusCode
            );
        }

        var messages = await _messageRepository.GetInboxAsync(
            currentSchoolId!,
            currentUserId!
        );

        var result = await MapToListDtoAsync(messages);

        return Result<List<MessageListDto>>.Success(
            result,
            "Gelen mesajlar başarıyla listelendi.",
            200
        );
    }

    public async Task<Result<List<MessageListDto>>> GetSentAsync(
        string? currentUserId,
        string? currentSchoolId
    )
    {
        var validation = ValidateUserAndSchool(currentUserId, currentSchoolId);
        if (validation is not null)
        {
            return Result<List<MessageListDto>>.Failure(
                validation.Message,
                validation.StatusCode
            );
        }

        var messages = await _messageRepository.GetSentAsync(
            currentSchoolId!,
            currentUserId!
        );

        var result = await MapToListDtoAsync(messages);

        return Result<List<MessageListDto>>.Success(
            result,
            "Gönderilen mesajlar başarıyla listelendi.",
            200
        );
    }

    public async Task<Result<List<MessageListDto>>> GetConversationAsync(
        string otherUserId,
        string? currentUserId,
        string? currentSchoolId
    )
    {
        var validation = ValidateUserAndSchool(currentUserId, currentSchoolId);
        if (validation is not null)
        {
            return Result<List<MessageListDto>>.Failure(
                validation.Message,
                validation.StatusCode
            );
        }

        if (string.IsNullOrWhiteSpace(otherUserId))
        {
            return Result<List<MessageListDto>>.Failure(
                "Diğer kullanıcı bilgisi bulunamadı.",
                400
            );
        }

        var otherUser = await _userRepository.GetByIdAsync(otherUserId);
        if (otherUser is null || otherUser.SchoolId != currentSchoolId)
        {
            return Result<List<MessageListDto>>.Failure(
                "Bu kullanıcı ile mesajlaşma yetkiniz yok.",
                403
            );
        }

        var messages = await _messageRepository.GetConversationAsync(
            currentSchoolId!,
            currentUserId!,
            otherUserId
        );

        var result = await MapToListDtoAsync(messages);

        return Result<List<MessageListDto>>.Success(
            result,
            "Konuşma başarıyla getirildi.",
            200
        );
    }

    public async Task<Result> SendAsync(
    CreateMessageDto dto,
    string? currentUserId,
    string? currentSchoolId
)
    {
        var validation = ValidateUserAndSchool(currentUserId, currentSchoolId);

        if (validation is not null)
        {
            return validation;
        }

        if (string.IsNullOrWhiteSpace(dto.ReceiverUserId))
        {
            return Result.Failure("Alıcı seçmelisiniz.", 400);
        }

        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            return Result.Failure("Mesaj başlığı boş olamaz.", 400);
        }

        if (string.IsNullOrWhiteSpace(dto.Content))
        {
            return Result.Failure("Mesaj içeriği boş olamaz.", 400);
        }

        var receiverUserId = dto.ReceiverUserId.Trim();

        if (IsClassroomTarget(receiverUserId))
        {
            return await SendToClassroomAsync(
                receiverUserId,
                dto.Title.Trim(),
                dto.Content.Trim(),
                currentUserId!,
                currentSchoolId!
            );
        }

        var receiver = await _userRepository.GetByIdAsync(receiverUserId);

        if (receiver is null)
        {
            return Result.Failure("Alıcı kullanıcı bulunamadı.", 404);
        }

        if (receiver.SchoolId != currentSchoolId)
        {
            return Result.Failure("Farklı okuldaki kullanıcıya mesaj gönderemezsiniz.", 403);
        }

        if (!receiver.IsActive)
        {
            return Result.Failure("Pasif kullanıcıya mesaj gönderemezsiniz.", 400);
        }

        if (!AllowedMessageRoles.Contains(receiver.RoleName.ToLower()))
        {
            return Result.Failure("Bu role sahip kullanıcıya mesaj gönderilemez.", 403);
        }

        var message = new Message
        {
            SchoolId = currentSchoolId!,
            SenderUserId = currentUserId!,
            ReceiverUserId = receiverUserId,
            Title = dto.Title.Trim(),
            Content = dto.Content.Trim(),
            IsRead = false,
            IsDeletedBySender = false,
            IsDeletedByReceiver = false
        };

        await _messageRepository.CreateAsync(message);

        return Result.Success("Mesaj başarıyla gönderildi.", 201);
    }

    public async Task<Result> MarkAsReadAsync(
        string id,
        string? currentUserId,
        string? currentSchoolId
    )
    {
        var validation = ValidateUserAndSchool(currentUserId, currentSchoolId);
        if (validation is not null)
        {
            return validation;
        }

        var message = await _messageRepository.GetByIdAsync(id);
        if (message is null)
        {
            return Result.Failure("Mesaj bulunamadı.", 404);
        }

        if (message.SchoolId != currentSchoolId || message.ReceiverUserId != currentUserId)
        {
            return Result.Failure("Bu mesajı okundu olarak işaretleme yetkiniz yok.", 403);
        }

        message.IsRead = true;
        message.UpdatedDate = DateTime.UtcNow;

        await _messageRepository.UpdateAsync(message);

        return Result.Success("Mesaj okundu olarak işaretlendi.");
    }

    public async Task<Result> DeleteAsync(
        string id,
        string? currentUserId,
        string? currentSchoolId
    )
    {
        var validation = ValidateUserAndSchool(currentUserId, currentSchoolId);
        if (validation is not null)
        {
            return validation;
        }

        var message = await _messageRepository.GetByIdAsync(id);
        if (message is null)
        {
            return Result.Failure("Mesaj bulunamadı.", 404);
        }

        if (message.SchoolId != currentSchoolId)
        {
            return Result.Failure("Bu mesajı silme yetkiniz yok.", 403);
        }

        if (message.SenderUserId == currentUserId)
        {
            message.IsDeletedBySender = true;
        }
        else if (message.ReceiverUserId == currentUserId)
        {
            message.IsDeletedByReceiver = true;
        }
        else
        {
            return Result.Failure("Bu mesajı silme yetkiniz yok.", 403);
        }

        message.UpdatedDate = DateTime.UtcNow;

        await _messageRepository.UpdateAsync(message);

        return Result.Success("Mesaj başarıyla silindi.");
    }

    private async Task<List<MessageListDto>> MapToListDtoAsync(List<Message> messages)
    {
        var result = new List<MessageListDto>();

        foreach (var message in messages)
        {
            var sender = await _userRepository.GetByIdAsync(message.SenderUserId);
            var receiver = await _userRepository.GetByIdAsync(message.ReceiverUserId);

            result.Add(new MessageListDto
            {
                Id = message.Id,
                SchoolId = message.SchoolId,
                SenderUserId = message.SenderUserId,
                SenderFullName = sender is null
                    ? "Bilinmeyen Kullanıcı"
                    : $"{sender.FirstName} {sender.LastName}",
                SenderRoleName = sender?.RoleName ?? "",
                ReceiverUserId = message.ReceiverUserId,
                ReceiverFullName = receiver is null
                    ? "Bilinmeyen Kullanıcı"
                    : $"{receiver.FirstName} {receiver.LastName}",
                ReceiverRoleName = receiver?.RoleName ?? "",
                Title = message.Title,
                Content = message.Content,
                IsRead = message.IsRead,
                CreatedDate = message.CreatedDate
            });
        }

        return result;
    }

    private async Task<List<MessageUserListDto>> GetClassroomMessageTargetsAsync(
    string currentUserId,
    string currentRoleName,
    string currentSchoolId
)
    {
        var roleName = currentRoleName.ToLower();

        if (roleName == "schooladmin")
        {
            var classrooms = await _classroomRepository.GetBySchoolIdAsync(currentSchoolId);

            return classrooms
                .Where(x => x.IsActive)
                .OrderBy(x => x.Grade)
                .ThenBy(x => x.Section)
                .Select(MapClassroomToMessageTarget)
                .ToList();
        }

        if (roleName == "teacher")
        {
            var teacher = await _teacherRepository.GetByUserIdAsync(currentUserId);

            if (teacher is null || !teacher.IsActive)
            {
                return new List<MessageUserListDto>();
            }

            var teacherLessons = await _teacherLessonRepository.GetByTeacherIdAsync(teacher.Id);
            var teacherLessonClassroomIds = teacherLessons
                .Where(x => x.SchoolId == currentSchoolId && x.IsActive)
                .Select(x => x.ClassroomId)
                .ToHashSet();

            var schoolClassrooms = await _classroomRepository.GetBySchoolIdAsync(currentSchoolId);

            return schoolClassrooms
                .Where(x =>
                    x.IsActive &&
                    (
                        teacherLessonClassroomIds.Contains(x.Id) ||
                        x.TeacherId == teacher.Id
                    )
                )
                .OrderBy(x => x.Grade)
                .ThenBy(x => x.Section)
                .Select(MapClassroomToMessageTarget)
                .ToList();
        }

        return new List<MessageUserListDto>();
    }

    private static MessageUserListDto MapClassroomToMessageTarget(Classroom classroom)
    {
        return new MessageUserListDto
        {
            UserId = $"{ClassroomTargetPrefix}{classroom.Id}",
            FullName = $"{classroom.Grade}-{classroom.Section}",
            RoleName = "Sınıf",
            Email = ""
        };
    }

    private async Task<Result> SendToClassroomAsync(
        string classroomTargetId,
        string title,
        string content,
        string currentUserId,
        string currentSchoolId
    )
    {
        var classroomId = GetClassroomIdFromTarget(classroomTargetId);

        if (string.IsNullOrWhiteSpace(classroomId))
        {
            return Result.Failure("Sınıf bilgisi bulunamadı.", 400);
        }

        var classroom = await _classroomRepository.GetByIdAsync(classroomId);

        if (classroom is null || classroom.SchoolId != currentSchoolId || !classroom.IsActive)
        {
            return Result.Failure("Mesaj gönderilecek sınıf bulunamadı.", 404);
        }

        var sender = await _userRepository.GetByIdAsync(currentUserId);

        if (sender is null || !sender.IsActive)
        {
            return Result.Failure("Gönderen kullanıcı bulunamadı.", 404);
        }

        var senderRoleName = sender.RoleName.ToLower();

        if (senderRoleName == "teacher")
        {
            var teacher = await _teacherRepository.GetByUserIdAsync(currentUserId);

            if (teacher is null || !teacher.IsActive)
            {
                return Result.Failure("Öğretmen bilgisi bulunamadı.", 404);
            }

            var teacherLessons = await _teacherLessonRepository.GetByTeacherIdAsync(teacher.Id);

            var teacherHasThisClassroom = teacherLessons.Any(x =>
                x.SchoolId == currentSchoolId &&
                x.ClassroomId == classroomId &&
                x.IsActive
            );

            var teacherIsClassroomTeacher = classroom.TeacherId == teacher.Id;

            if (!teacherHasThisClassroom && !teacherIsClassroomTeacher)
            {
                return Result.Failure(
                    "Bu sınıfa toplu mesaj gönderme yetkiniz yok.",
                    403
                );
            }
        }
        else if (senderRoleName != "schooladmin")
        {
            return Result.Failure(
                "Sınıfa toplu mesaj gönderme yetkiniz yok.",
                403
            );
        }

        var students = await _studentRepository.GetByClassroomIdAsync(classroomId);

        var activeStudents = students
            .Where(x =>
                x.SchoolId == currentSchoolId &&
                x.IsActive &&
                !string.IsNullOrWhiteSpace(x.UserId)
            )
            .ToList();

        if (!activeStudents.Any())
        {
            return Result.Failure("Bu sınıfta mesaj gönderilecek aktif öğrenci bulunamadı.", 404);
        }

        foreach (var student in activeStudents)
        {
            var message = new Message
            {
                SchoolId = currentSchoolId,
                SenderUserId = currentUserId,
                ReceiverUserId = student.UserId,
                Title = title,
                Content = content,
                IsRead = false,
                IsDeletedBySender = false,
                IsDeletedByReceiver = false
            };

            await _messageRepository.CreateAsync(message);
        }

        return Result.Success(
            $"{activeStudents.Count} öğrenciye mesaj başarıyla gönderildi.",
            201
        );
    }

    private static bool IsClassroomTarget(string receiverUserId)
    {
        return receiverUserId.StartsWith(
            ClassroomTargetPrefix,
            StringComparison.OrdinalIgnoreCase
        );
    }

    private static string GetClassroomIdFromTarget(string receiverUserId)
    {
        return receiverUserId[ClassroomTargetPrefix.Length..];
    }

    private static Result? ValidateUserAndSchool(string? currentUserId, string? currentSchoolId)
    {
        if (string.IsNullOrWhiteSpace(currentUserId))
        {
            return Result.Failure("Kullanıcı bilgisi bulunamadı.", 400);
        }

        if (string.IsNullOrWhiteSpace(currentSchoolId))
        {
            return Result.Failure("Okul bilgisi bulunamadı.", 400);
        }

        return null;
    }

    private static bool IsAllowedRole(string? roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return false;
        }

        return AllowedMessageRoles.Contains(roleName.ToLower());
    }

    private static int GetRoleOrder(string roleName)
    {
        return roleName.ToLower() switch
        {
            "schooladmin" => 1,
            "teacher" => 2,
            "officer" => 3,
            _ => 99
        };
    }
}