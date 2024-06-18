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
}