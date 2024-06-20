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
        
        [HttpPatch("Accept/{id}")]
        public IActionResult Accept(int id, Answer answer)
        {
            if (!HttpContext.Session.TryGetValue("AuthToken", out byte[] authToken))
            {
                return Unauthorized("You must be logged in to accept an answer.");
            }

            // int userId = HttpContext.Session.GetInt32("userId").GetValueOrDefault(); // ez itt nem jó 
            int userId = answer.User_id;
            Console.Write(userId); // 00!!!!
            
            var repository = new AnswerRepository(new NpgsqlConnection(_connectionString));
            try
            {
                // Ensure the logged-in user is the one who asked the question
                if (!repository.CanAcceptAnswer(id, userId))  // itt nem jó értékek kerülnek be
                {
                    return BadRequest("You can only accept an answer to your own question.");
                }

                repository.AcceptAnswer(id);
                return Ok("Answer accepted successfully.");
            }
            catch (NpgsqlException ex)
            {
                Log.Error($"Database error: {ex.Message}");
                throw new Exception($"SOMETHING WRONG HERE {ex}");
                // return StatusCode(500, "A database error occurred while accepting the answer.");
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message}");
                throw new Exception($"HUHUHUHUHU    {ex}");
                return StatusCode(500, "An error occurred while accepting the answer.");
            }
        }

        [HttpPost]
        public IActionResult Create(Answer answer)
        {
            if (!HttpContext.Session.TryGetValue("AuthToken", out byte[] authToken))
            {
                return Unauthorized("You must be logged in to post an answer.");
            }

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