using System.Data;
namespace AskMate.Model.Repositories;
using Npgsql;

public class UserRepository
{
    private readonly NpgsqlConnection _connection;

    public UserRepository(NpgsqlConnection connection)
    {
        _connection = connection;
    }
    
    public User AuthenticateUser(string usernameOrEmail, string password)
    {
        _connection.Open();
        var command = new NpgsqlCommand("SELECT * FROM users WHERE (username = @usernameOrEmail OR email = @usernameOrEmail) AND password = @password", _connection);
        command.Parameters.AddWithValue("@usernameOrEmail", usernameOrEmail);
        command.Parameters.AddWithValue("@password", password);

        using (var reader = command.ExecuteReader())
        {
            if (reader.Read())
            {
                return new User
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Username = reader.GetString(reader.GetOrdinal("username")),
                    Email = reader.GetString(reader.GetOrdinal("email")),
                    Password = reader.GetString(reader.GetOrdinal("password")),  // Note: Storing passwords in plain text is not recommended.
                    Registration_time = reader.GetDateTime(reader.GetOrdinal("registration_time"))
                };
            }
        }

        _connection.Close();
        return null;
    }

    public int Create(User user)
    {
        _connection.Open();
        var adapter = new NpgsqlDataAdapter("INSERT INTO users (username, email, password, registration_time)" +
                                            "VALUES (:username, :email, :password, :registration_time) RETURNING id", _connection);
        adapter.SelectCommand?.Parameters.AddWithValue(":username", user.Username);
        adapter.SelectCommand?.Parameters.AddWithValue(":email", user.Email);
        adapter.SelectCommand?.Parameters.AddWithValue(":password", user.Password);
        adapter.SelectCommand?.Parameters.AddWithValue(":registration_time", user.Registration_time);

        var lasInsertId = (int)adapter.SelectCommand?.ExecuteScalar();
        _connection.Close();

        return lasInsertId;
    }
}