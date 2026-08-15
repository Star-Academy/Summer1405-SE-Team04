using ASP.Models;
using Xunit;

namespace AspTests.Fakes;

public static class StudentFakes
{
    public static List<Student> CreateStudents() =>
    [
        new Student
        {
            StudentNumber = "40012001",
            FirstName = "Ali",
            LastName = "Almasi",
            Grade = 17.5f,
            IsMale = true,
            DateOfBirth = new DateTime(2002, 4, 12),
            LeftUnitsCount = 24
        },
        new Student
        {
            StudentNumber = "40012002",
            FirstName = "Sara",
            LastName = "Karimi",
            Grade = 19.25f,
            IsMale = false,
            DateOfBirth = new DateTime(2003, 1, 30),
            LeftUnitsCount = 8
        },
        new Student
        {
            StudentNumber = "40012003",
            FirstName = "Reza",
            LastName = "Ahmadi",
            Grade = 13f,
            IsMale = true,
            DateOfBirth = new DateTime(2001, 11, 5),
            LeftUnitsCount = 46
        }
    ];

    public static TheoryData<Student> IndividualStudentsData
    {
        get
        {
            var data = new TheoryData<Student>();
            data.AddRange(CreateStudents());
            return data;
        }
    }

    public static Student CreateStudent(string studentNumber = "40012009") =>
        new Student
        {
            StudentNumber = studentNumber,
            FirstName = "Maryam",
            LastName = "Hosseini",
            Grade = 15.75f,
            IsMale = false,
            DateOfBirth = new DateTime(2002, 8, 21),
            LeftUnitsCount = 32
        };
    
}
