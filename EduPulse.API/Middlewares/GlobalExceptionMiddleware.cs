using System.Net;
using System.Text.Json;
using MongoDB.Driver;

namespace EduPulse.API.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Beklenmeyen bir hata oluştu.");

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var statusCode = exception switch
        {
            MongoWriteException mongoEx when mongoEx.WriteError.Category == ServerErrorCategory.DuplicateKey
                => HttpStatusCode.Conflict,

            ArgumentException
                => HttpStatusCode.BadRequest,

            _ => HttpStatusCode.InternalServerError
        };

        var message = exception switch
        {
            MongoWriteException mongoEx when mongoEx.WriteError.Category == ServerErrorCategory.DuplicateKey
                => GetDuplicateKeyMessage(mongoEx),

            ArgumentException argEx
                => argEx.Message,

            _ => "Beklenmeyen bir hata oluştu."
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            Success = false,
            Message = message,
            StatusCode = context.Response.StatusCode,
            Detail = _environment.IsDevelopment() ? exception.Message : null
        };

        var json = JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(json);
    }

    private static string GetDuplicateKeyMessage(MongoWriteException exception)
    {
        var errorMessage = exception.Message;

        if (errorMessage.Contains("UX_Students_SchoolId_SchoolNumber"))
            return "Bu okul numarası bu okulda zaten kullanılıyor.";

        if (errorMessage.Contains("UX_Lessons_SchoolId_Name"))
            return "Bu ders adı bu okulda zaten mevcut.";

        if (errorMessage.Contains("UX_Classrooms_SchoolId_Grade_Section"))
            return "Bu sınıf bu okulda zaten mevcut.";

        if (errorMessage.Contains("UX_Teachers_SchoolId_PhoneNumber"))
            return "Bu öğretmen telefonu bu okulda zaten kullanılıyor.";

        if (errorMessage.Contains("UX_Teachers_SchoolId_Email"))
            return "Bu öğretmen e-posta adresi bu okulda zaten kullanılıyor.";

        if (errorMessage.Contains("UX_Parents_SchoolId_PhoneNumber"))
            return "Bu veli telefonu bu okulda zaten kullanılıyor.";

        if (errorMessage.Contains("UX_Parents_SchoolId_Email"))
            return "Bu veli e-posta adresi bu okulda zaten kullanılıyor.";

        if (errorMessage.Contains("UX_Schools_Email"))
            return "Bu e-posta adresiyle kayıtlı bir okul zaten var.";

        if (errorMessage.Contains("UX_TeacherLessons_SchoolId_TeacherId_LessonId_ClassroomId"))
            return "Bu öğretmen bu sınıfa bu ders için zaten atanmış.";

        if (errorMessage.Contains("UX_StudentGrades_SchoolId_StudentId_LessonId"))
            return "Bu öğrencinin bu derse ait not kaydı zaten mevcut.";

        return "Bu kayıt sistemde zaten mevcut.";
    }
}