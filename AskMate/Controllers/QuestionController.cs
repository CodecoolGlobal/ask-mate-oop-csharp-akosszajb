using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace AskMate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly string _connectionString;

        public QuestionController(IWebHostEnvironment env)
        {
            string filePath = Path.Combine(env.ContentRootPath, "ConnectionString.txt");

            // Read the connection string from the file
            _connectionString = System.IO.File.ReadAllText(filePath).Trim();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok("No questions were asked yet.");
        }
    }
}