using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using AskMate.Model;
using AskMate.Model.Repositories;
using Npgsql;

namespace AskMate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly string _connectionString;
        public QuestionController(IWebHostEnvironment env)
        {
            string filePath = Path.Combine("Model", "ConnectionString.txt");
                
            // Read the connection string from the file
            _connectionString = System.IO.File.ReadAllText(filePath).Trim();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var repository = new QuestionRepository(new NpgsqlConnection(_connectionString));
            return Ok(repository.GetAll());
        }
        
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var repository = new QuestionRepository(new NpgsqlConnection(_connectionString));

            return Ok(repository.GetById(id));
        }
        
        [HttpPost()]
        public IActionResult Create(Question question)
        {
            var repository = new QuestionRepository(new NpgsqlConnection(_connectionString));
            return Ok(repository.Create(question));
        }
    }
    
}