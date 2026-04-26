using EduPulse.Business.Abstracts;
using EduPulse.Business.Concretes;
using EduPulse.Repository.Abstracts;
using EduPulse.Repository.Concretes;
using EduPulse.Repository.Context;
using EduPulse.Repository.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings")
);

builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddScoped<ISchoolRepository, SchoolRepository>();
builder.Services.AddScoped<ISchoolService, SchoolService>();

builder.Services.AddScoped<ILessonRepository, LessonRepository>();
builder.Services.AddScoped<ILessonService, LessonService>();

builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<ITeacherService, TeacherService>();

builder.Services.AddScoped<IClassroomRepository, ClassroomRepository>();
builder.Services.AddScoped<IClassroomService, ClassroomService>();

builder.Services.AddScoped<ITeacherLessonRepository, TeacherLessonRepository>();
builder.Services.AddScoped<ITeacherLessonService, TeacherLessonService>();

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();

builder.Services.AddScoped<IParentRepository, ParentRepository>();
builder.Services.AddScoped<IParentService, ParentService>();

builder.Services.AddScoped<IStudentGradeRepository, StudentGradeRepository>();
builder.Services.AddScoped<IStudentGradeService, StudentGradeService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();