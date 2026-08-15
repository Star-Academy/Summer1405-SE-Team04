using ASP.Exceptions;
using ASP.Models;
using ASP.Services;
using Microsoft.AspNetCore.Mvc;

namespace ASP.Controllers;

[ApiController]
[Route("[controller]")]
public class StudentController(IStudentService studentService) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Student>> List([FromQuery] string dbName)
    {
        try
        {
            return studentService.ListStudents(dbName).ToList();
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{studentNumber}")]
    public ActionResult<Student> Retrieve([FromQuery] string dbName, string studentNumber)
    {
        try
        {
            return studentService.GetStudent(dbName, studentNumber);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpDelete("{studentNumber}")]
    public IActionResult Delete([FromQuery] string dbName, string studentNumber)
    {
        try
        {
            studentService.DeleteStudent(dbName, studentNumber);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost]
    public ActionResult<Student> Create([FromQuery] string dbName, [FromBody] Student student)
    {
        try
        {
            studentService.CreateStudent(dbName, student);
            return student;
        }
        catch (ConflictException e)
        {
            return Conflict(e.Message);
        }
    }

    [HttpPut("{studentNumber}")]
    public ActionResult<Student> Update([FromQuery] string dbName, [FromBody] Student student, string studentNumber)
    {
        try
        {
            studentService.UpdateStudent(dbName, studentNumber, student);
            return student;
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (NotAllowedException e)
        {
            return BadRequest(e.Message);
        }
    }
}