using EduPulse.Entities.Classrooms;
using EduPulse.Entities.Lessons;
using EduPulse.Entities.Parents;
using EduPulse.Entities.Schools;
using EduPulse.Entities.StudentGrades;
using EduPulse.Entities.Students;
using EduPulse.Entities.TeacherLessons;
using EduPulse.Entities.Teachers;
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
    }

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
}