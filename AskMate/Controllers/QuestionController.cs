using Microsoft.AspNetCore.Mvc;

namespace AskMate.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok("No questions were asked yet.");
    }
}