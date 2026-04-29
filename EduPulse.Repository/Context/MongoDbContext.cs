using EduPulse.Entities.Classrooms;
using EduPulse.Entities.Lessons;
using EduPulse.Entities.Parents;
using EduPulse.Entities.Roles;
using EduPulse.Entities.Schools;
using EduPulse.Entities.StudentGrades;
using EduPulse.Entities.Students;
using EduPulse.Entities.TeacherLessons;
using EduPulse.Entities.Teachers;
using EduPulse.Entities.Users;
using EduPulse.Repository.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EduPulse.Repository.Context;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        MongoClient client = new(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);

        CreateIndexes();
    }
    public IMongoCollection<Role> Roles =>
    _database.GetCollection<Role>("Roles");

    public IMongoCollection<User> Users =>
    _database.GetCollection<User>("Users");

    public IMongoCollection<School> Schools =>
        _database.GetCollection<School>("Schools");

    public IMongoCollection<Lesson> Lessons =>
        _database.GetCollection<Lesson>("Lessons");

    public IMongoCollection<Teacher> Teachers =>
        _database.GetCollection<Teacher>("Teachers");

    public IMongoCollection<Classroom> Classrooms =>
        _database.GetCollection<Classroom>("Classrooms");

    public IMongoCollection<TeacherLesson> TeacherLessons =>
        _database.GetCollection<TeacherLesson>("TeacherLessons");

    public IMongoCollection<Student> Students =>
        _database.GetCollection<Student>("Students");

    public IMongoCollection<Parent> Parents =>
        _database.GetCollection<Parent>("Parents");

    public IMongoCollection<StudentGrade> StudentGrades =>
        _database.GetCollection<StudentGrade>("StudentGrades");

    private void CreateIndexes()
    {
        CreateUserIndexes();
        CreateSchoolIndexes();
        CreateLessonIndexes();
        CreateTeacherIndexes();
        CreateClassroomIndexes();
        CreateStudentIndexes();
        CreateParentIndexes();
        CreateTeacherLessonIndexes();
        CreateStudentGradeIndexes();
    }

    private void CreateUserIndexes()
    {
        Users.Indexes.CreateOne(new CreateIndexModel<User>(
            Builders<User>.IndexKeys.Ascending(x => x.Email),
            new CreateIndexOptions
            {
                Unique = true,
                Name = "UX_Users_Email"
            }));
    }

    private void CreateSchoolIndexes()
    {
        Schools.Indexes.CreateOne(new CreateIndexModel<School>(
            Builders<School>.IndexKeys.Ascending(x => x.Email),
            new CreateIndexOptions
            {
                Unique = true,
                Sparse = true,
                Name = "UX_Schools_Email"
            }));

       Schools.Indexes.CreateOne(new CreateIndexModel<School>(
           Builders<School>.IndexKeys.Ascending(x => x.PhoneNumber),
           new CreateIndexOptions
           {
               Unique = true,
               Sparse = true,
               Name = "UX_Schools_PhoneNumber"
           }));
    }

    private void CreateLessonIndexes()
    {
        Lessons.Indexes.CreateOne(new CreateIndexModel<Lesson>(
            Builders<Lesson>.IndexKeys
                .Ascending(x => x.SchoolId)
                .Ascending(x => x.NormalizedName),
            new CreateIndexOptions<Lesson>
            {
                Unique = true,
                Name = "UX_Lessons_SchoolId_NormalizedName_Active",
                PartialFilterExpression = Builders<Lesson>.Filter.Eq(x => x.IsActive, true)
            }));

        Lessons.Indexes.CreateOne(new CreateIndexModel<Lesson>(
            Builders<Lesson>.IndexKeys
                .Ascending(x => x.SchoolId)
                .Ascending(x => x.IsActive),
            new CreateIndexOptions
            {
                Name = "IX_Lessons_SchoolId_IsActive"
            }));
    }

    private void CreateTeacherIndexes()
    {
        Teachers.Indexes.CreateOne(new CreateIndexModel<Teacher>(
            Builders<Teacher>.IndexKeys
                .Ascending(x => x.SchoolId)
                .Ascending(x => x.PhoneNumber),
            new CreateIndexOptions
            {
                Unique = true,
                Name = "UX_Teachers_SchoolId_PhoneNumber"
            }));

        Teachers.Indexes.CreateOne(new CreateIndexModel<Teacher>(
            Builders<Teacher>.IndexKeys
                .Ascending(x => x.SchoolId)
                .Ascending(x => x.Email),
            new CreateIndexOptions
            {
                Unique = true,
                Sparse = true,
                Name = "UX_Teachers_SchoolId_Email"
            }));

        Teachers.Indexes.CreateOne(new CreateIndexModel<Teacher>(
            Builders<Teacher>.IndexKeys
                .Ascending(x => x.SchoolId)
                .Ascending(x => x.IsActive),
            new CreateIndexOptions
            {
                Name = "IX_Teachers_SchoolId_IsActive"
            }));
    }

    private void CreateClassroomIndexes()
    {
        Classrooms.Indexes.CreateOne(new CreateIndexModel<Classroom>(
            Builders<Classroom>.IndexKeys
                .Ascending(x => x.SchoolId)
                .Ascending(x => x.Grade)
                .Ascending(x => x.Section),
            new CreateIndexOptions
            {
                Unique = true,
                Name = "UX_Classrooms_SchoolId_Grade_Section"
            }));

        Classrooms.Indexes.CreateOne(new CreateIndexModel<Classroom>(
            Builders<Classroom>.IndexKeys
                .Ascending(x => x.SchoolId)
                .Ascending(x => x.IsActive),
            new CreateIndexOptions
            {
                Name = "IX_Classrooms_SchoolId_IsActive"
            }));
    }

    private void CreateStudentIndexes()
    {
        Students.Indexes.CreateOne(new CreateIndexModel<Student>(
            Builders<Student>.IndexKeys
                .Ascending(x => x.SchoolId)
                .Ascending(x => x.SchoolNumber),
            new CreateIndexOptions
            {
                Unique = true,
                Name = "UX_Students_SchoolId_SchoolNumber"
            }));

        Students.Indexes.CreateOne(new CreateIndexModel<Student>(
            Builders<Student>.IndexKeys
                .Ascending(x => x.SchoolId)
                .Ascending(x => x.IsActive),
            new CreateIndexOptions
            {
                Name = "IX_Students_SchoolId_IsActive"
            }));

        Students.Indexes.CreateOne(new CreateIndexModel<Student>(
            Builders<Student>.IndexKeys
                .Ascending(x => x.ClassroomId)
                .Ascending(x => x.IsActive),
            new CreateIndexOptions
            {
                Name = "IX_Students_ClassroomId_IsActive"
            }));
    }

    private void CreateParentIndexes()
    {
        Parents.Indexes.CreateOne(new CreateIndexModel<Parent>(
            Builders<Parent>.IndexKeys
                .Ascending(x => x.SchoolId)
                .Ascending(x => x.PhoneNumber),
            new CreateIndexOptions
            {
                Unique = true,
                Name = "UX_Parents_SchoolId_PhoneNumber"
            }));

        Parents.Indexes.CreateOne(new CreateIndexModel<Parent>(
            Builders<Parent>.IndexKeys
                .Ascending(x => x.SchoolId)
                .Ascending(x => x.Email),
            new CreateIndexOptions
            {
                Unique = true,
                Sparse = true,
                Name = "UX_Parents_SchoolId_Email"
            }));

        Parents.Indexes.CreateOne(new CreateIndexModel<Parent>(
            Builders<Parent>.IndexKeys
                .Ascending(x => x.SchoolId)
                .Ascending(x => x.IsActive),
            new CreateIndexOptions
            {
                Name = "IX_Parents_SchoolId_IsActive"
            }));
    }

    private void CreateTeacherLessonIndexes()
    {
        TeacherLessons.Indexes.CreateOne(new CreateIndexModel<TeacherLesson>(
            Builders<TeacherLesson>.IndexKeys
                .Ascending(x => x.SchoolId)
                .Ascending(x => x.TeacherId)
                .Ascending(x => x.LessonId)
                .Ascending(x => x.ClassroomId),
            new CreateIndexOptions
            {
                Unique = true,
                Name = "UX_TeacherLessons_SchoolId_TeacherId_LessonId_ClassroomId"
            }));

        TeacherLessons.Indexes.CreateOne(new CreateIndexModel<TeacherLesson>(
            Builders<TeacherLesson>.IndexKeys
                .Ascending(x => x.SchoolId)
                .Ascending(x => x.IsActive),
            new CreateIndexOptions
            {
                Name = "IX_TeacherLessons_SchoolId_IsActive"
            }));
    }

    private void CreateStudentGradeIndexes()
    {
        StudentGrades.Indexes.CreateOne(new CreateIndexModel<StudentGrade>(
            Builders<StudentGrade>.IndexKeys
                .Ascending(x => x.SchoolId)
                .Ascending(x => x.StudentId)
                .Ascending(x => x.LessonId),
            new CreateIndexOptions
            {
                Unique = true,
                Name = "UX_StudentGrades_SchoolId_StudentId_LessonId"
            }));

        StudentGrades.Indexes.CreateOne(new CreateIndexModel<StudentGrade>(
            Builders<StudentGrade>.IndexKeys
                .Ascending(x => x.SchoolId)
                .Ascending(x => x.IsActive),
            new CreateIndexOptions
            {
                Name = "IX_StudentGrades_SchoolId_IsActive"
            }));
    }
}