using Microsoft.AspNetCore.Mvc;
using AskMate.Model;
using AskMate.Model.Repositories;
using Npgsql;
using Serilog;

namespace AskMate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnswerController : ControllerBase
    {
        private readonly string _connectionString;

        public AnswerController(IWebHostEnvironment env)
        {
            string filePath = Path.Combine("Model", "ConnectionString.txt");
            _connectionString = System.IO.File.ReadAllText(filePath).Trim();
        }

        [HttpPost]
        public IActionResult Create(Answer answer)
        {
            if (answer == null)
            {
                return BadRequest("Invalid answer data.");
            }

            var repository = new AnswerRepository(new NpgsqlConnection(_connectionString));

            try
            {
                var newAnswerId = repository.Create(answer);
                return Ok(newAnswerId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NpgsqlException ex)
            {
                Log.Error($"Database error: {ex.Message}");
                return StatusCode(500, "A database error occurred while creating the answer.");
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while creating the answer.");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var repository = new AnswerRepository(new NpgsqlConnection(_connectionString));
            
            try
            {
                repository.Delete(id);
                return Ok();
            }
            catch (NpgsqlException ex)
            {
                Log.Error($"Database error: {ex.Message}");
                return StatusCode(500, "A database error occurred while deleting the answer.");
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while deleting the answer.");
            }
        }
    }
}