using ASP.Models;

namespace ASP.Services;

public interface IStudentService
{
    IEnumerable<Student> ListStudents(string dbName);
    Student GetStudent(string dbName, string studentNumber);
    void CreateStudent(string dbName, Student student);
    void UpdateStudent(string dbName, string studentNumber, Student student);
    void DeleteStudent(string dbName, string studentNumber);
}