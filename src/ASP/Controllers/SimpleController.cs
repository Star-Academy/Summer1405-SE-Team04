using Microsoft.AspNetCore.Mvc;


namespace Controllers;
[ApiController]
[Route("[controller]/[Action]")]
public class SimpleController : ControllerBase
{
    [HttpGet] // TODO : Test without it
    public string GetSimple()
    {
        return "Hello from SimpleController!";
    }
}