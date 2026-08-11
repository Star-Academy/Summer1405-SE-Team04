using SqlKata.Execution;
using Models;
using Exceptions;
using Providers;
namespace Services;

public class StudentService(IQueryFactoryProvider queryFactoryProvider) : IStudentService
{
    public void CreateStudent(string dbName, Student student)
    {
        var db = queryFactoryProvider.GetQueryFactory(dbName);
        if (db.Query().From("Student").Where("StudentNumber", student.StudentNumber).Exists())
            throw new ConflictException("Student with this username already exists.");
        db.Query("Student").Insert(student);
    }

    public void DeleteStudent(string dbName, string studentNumber)
    {
        var db = queryFactoryProvider.GetQueryFactory(dbName);
        if (!db.Query("Student").Where("StudentNumber", studentNumber).Exists())
            throw new NotFoundException("The Student doesn't exist!");
        db.Query("Student").Where("StudentNumber", studentNumber).Delete();
    }

    public Student GetStudent(string dbName, string studentNumber)
    {
        var db = queryFactoryProvider.GetQueryFactory(dbName);
        var student = db.Query("Student").Where("StudentNumber", studentNumber).FirstOrDefault();
        return student ?? throw new NotFoundException("Student doesn't exist!");
    }

    public IEnumerable<Student> ListStudents(string dbName)
    {
        var db = queryFactoryProvider.GetQueryFactory(dbName);
        return db.Query("Student").Get<Student>();
    }

    public void UpdateStudent(string dbName, string studentNumber, Student student)
    {
        var db = queryFactoryProvider.GetQueryFactory(dbName);
        
        if (student.StudentNumber != studentNumber)
            throw new NotAllowedException("you can't change the student number!");
        if (!db.Query("Student").Where("StudentNumber", studentNumber).Exists())
            throw new NotFoundException("The Student doesn't exist!");
        db.Query("Student").Where("StudentNumber", studentNumber).Update(student);
    }
}