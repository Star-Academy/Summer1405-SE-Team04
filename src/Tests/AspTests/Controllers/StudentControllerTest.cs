using ASP.Controllers;
using ASP.Exceptions;
using ASP.Models;
using ASP.Services;
using AspTests.Fakes;
using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace AspTests.Controllers;

public class StudentControllerTest
{
    private const string DbName = "postgres";

    private readonly List<Student> _students;
    private readonly StudentController _sut;
    private readonly IStudentService _studentService;

    public StudentControllerTest()
    {
        _students = StudentFakes.CreateStudents();
        _studentService = Substitute.For<IStudentService>();

        _studentService.ListStudents(Arg.Any<string>())
            .Returns(_ => _students);

        _studentService.GetStudent(Arg.Any<string>(), Arg.Any<string>())
            .Returns(callInfo => Find(callInfo.ArgAt<string>(1))
                             ?? throw new NotFoundException("The Student doesn't exist!"));
        
        _studentService.CreateStudent(Arg.Any<string>(), Arg.Any<Student>());

        _studentService.When(s => s.CreateStudent(Arg.Any<string>(), Arg.Any<Student>()))
            .Do(callInfo =>
            {
                var student = callInfo.ArgAt<Student>(1);
                if (Find(student.StudentNumber) is not null)
                    throw new ConflictException("Student with this username already exists.");
                _students.Add(student);
            });
        
        _studentService.When(s => s.UpdateStudent(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Student>()))
            .Do(callInfo =>
            {
                var studentNumber = callInfo.ArgAt<string>(1);
                var student = callInfo.ArgAt<Student>(2);
                if (student.StudentNumber != studentNumber)
                    throw new NotAllowedException("you can't change the student number!");
                var existing = Find(studentNumber)
                               ?? throw new NotFoundException("The Student doesn't exist!");
                _students[_students.IndexOf(existing)] = student;
            });

        _studentService.When(s => s.DeleteStudent(Arg.Any<string>(), Arg.Any<string>()))
            .Do(callInfo =>
            {
                var existing = Find(callInfo.ArgAt<string>(1)) 
                               ?? throw new NotFoundException("The Student doesn't exist!");
                _students.RemoveAll(s => s.StudentNumber == existing.StudentNumber);
            });

        _sut = new StudentController(_studentService);
    }

    private Student? Find(string studentNumber) =>
        _students.FirstOrDefault(s => s.StudentNumber == studentNumber);

    [Fact]
    public void List_ShouldReturnAllStudents_WhenCalled()
    {
        // Arrange

        // Act
        var result = _sut.List(DbName);

        // Assert
        result.Value.Should().BeEquivalentTo(_students);
    }

    [Theory]
    [MemberData(nameof(StudentFakes.IndividualStudentsData),  MemberType = typeof(StudentFakes))]
    public void Retrieve_ShouldReturnCorrectStudent_WhenCalled(Student queryStudent)
    {
        // Arrange
        var expected = _students.FirstOrDefault(s => s.StudentNumber == queryStudent.StudentNumber);

        // Act
        var result = _sut.Retrieve(DbName, queryStudent.StudentNumber);

        // Assert
        result.Value.Should().BeSameAs(expected);
    }

    [Fact]
    public void Create_ShouldAddStudent_WhenCalled()
    {
        // Arrange
        var student = StudentFakes.CreateStudent();

        // Act
        var result = _sut.Create(DbName, student);

        // Assert
        result.Value.Should().BeSameAs(student);
        _students.Should().Contain(student);
    }

    [Fact]
    public void Delete_ShouldRemoveStudent_WhenCalled()
    {
        // Arrange
        var target = _students[1];

        // Act
        var result = _sut.Delete(DbName, target.StudentNumber);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _students.Should().NotContain(target);
    }

    [Fact]
    public void Update_ShouldUpdateStudent_WhenCalled()
    {
        // Arrange
        var target = _students[0];
        var updated = StudentFakes.CreateStudent(target.StudentNumber);

        // Act
        var result = _sut.Update(DbName, updated, target.StudentNumber);

        // Assert
        result.Value.Should().BeSameAs(updated);
        Find(target.StudentNumber).Should().BeSameAs(updated);
        _students.Should().HaveCount(3);
    }

    [Fact]
    public void List_ShouldReturnEmptyList_WhenNoStudentExists()
    {
        // Arrange
        _students.Clear();

        // Act
        var result = _sut.List(DbName);

        // Assert
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public void Retrieve_ShouldReturnNotFound_WhenCalledWithUnknownStudentNumber()
    {
        // Arrange
        const string unknownStudentNumber = "00000000";

        // Act
        var result = _sut.Retrieve(DbName, unknownStudentNumber);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void Delete_ShouldReturnNotFound_WhenCalledWithUnknownStudentNumber()
    {
        // Arrange
        const string unknownStudentNumber = "00000000";

        // Act
        var result = _sut.Delete(DbName, unknownStudentNumber);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void Update_ShouldReturnNotFound_WhenCalledWithUnknownStudentNumber()
    {
        // Arrange
        const string unknownStudentNumber = "00000000";
        var updated = StudentFakes.CreateStudent(unknownStudentNumber);

        // Act
        var result = _sut.Update(DbName, updated, unknownStudentNumber);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void Create_ShouldReturnConflict_WhenCalledWithExistingStudentNumber()
    {
        // Arrange
        var duplicate = StudentFakes.CreateStudent(_students[0].StudentNumber);

        // Act
        var result = _sut.Create(DbName, duplicate);

        // Assert
        result.Result.Should().BeOfType<ConflictObjectResult>();
        _students.Should().HaveCount(3);
    }

    [Fact]
    public void Update_ShouldReturnBadRequest_WhenCalledWithDifferentStudentNumber()
    {
        // Arrange
        var target = _students[0];
        var updated = StudentFakes.CreateStudent("99999999");

        // Act
        var result = _sut.Update(DbName, updated, target.StudentNumber);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        Find(target.StudentNumber).Should().NotBeNull();
        Find("99999999").Should().BeNull();
    }
}
