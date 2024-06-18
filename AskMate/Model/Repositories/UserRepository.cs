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