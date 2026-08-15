namespace ASP.Models;

public class Student
{
    public string StudentNumber {get; set;} = string.Empty;
    public float Grade {get; set;}
    public string FirstName {get; set;} = string.Empty;
    public string LastName {get; set;} = string.Empty;
    public bool IsMale {get; set;}
    public DateTime DateOfBirth {get; set;}
    public int LeftUnitsCount {get; set;}
}