using System.Data;

namespace AskMate.Model.Repositories;
using Npgsql;

public class QuestionRepository
{
    private readonly NpgsqlConnection _connection;

    public QuestionRepository(NpgsqlConnection connection)
    {
        _connection = connection;
    }
    
    public List<Question> GetAll()
    {
        _connection.Open();

        var adapter = new NpgsqlDataAdapter("SELECT * FROM questions ORDER BY submission_time ASC", _connection);

        var dataSet = new DataSet();
        adapter.Fill(dataSet);
        var table = dataSet.Tables[0];

        var queryResult = new List<Question>();

        foreach (DataRow row in table.Rows)
        {
            queryResult.Add(new Question
            {
                Id = (int)row["id"],
                Title = (string)row["title"],
                Description = (string)row["description"],
                Submission_time = (DateTime)row["submission_time"]
            });
        }
        _connection.Close();
        return queryResult;
    }
    
    public Question GetById(int id)
    {
        _connection.Open();
        var adapter = new NpgsqlDataAdapter("SELECT * FROM questions WHERE id = :id", _connection);
        adapter.SelectCommand?.Parameters.AddWithValue(":id", id);

        var dataSet = new DataSet();
        adapter.Fill(dataSet);
        var table = dataSet.Tables[0];

        if (table.Rows.Count > 0)
        {
            DataRow row = table.Rows[0];
            return new Question
            {
                Id = (int)row["id"],
                Title = (string)row["title"],
                Description = (string)row["description"],
                Submission_time = (DateTime)row["submission_time"]
            };
        }
        _connection.Close();

        return null;
    }
}