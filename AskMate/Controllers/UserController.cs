using Microsoft.AspNetCore.Mvc;
using AskMate.Model;
using AskMate.Model.Repositories;
using Npgsql;

namespace AskMate.Controllers;

[ApiController]
[Route("[controller]")]

public class UserController : ControllerBase
{
    private readonly string _connectionString;
    
    public UserController(IWebHostEnvironment env)
    {
        string filePath = Path.Combine("Model", "ConnectionString.txt");

        _connectionString = System.IO.File.ReadAllText(filePath).Trim();
    }

    [HttpPost()]
    public IActionResult Create(User user)
    {
        var repository = new UserRepository(new NpgsqlConnection(_connectionString));
        return Ok(repository.Create(user));
    }
    
    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginModel login)
    {
        var repository = new UserRepository(new NpgsqlConnection(_connectionString));
        var user = repository.AuthenticateUser(login.UsernameOrEmail, login.Password);
        if (user != null)
        {
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());  // This is a placeholder. In production, use a secure method.
            // Store this token in a database or cache with an expiration time
            HttpContext.Session.SetString("AuthToken", token);
            return Ok(new { token = token, message = "Login successful!" });
        }
        else
        {
            return Unauthorized(new { message = "Username or password is incorrect" });
        }
    }

}