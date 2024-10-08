using Npgsql;
using Serilog;

namespace AskMate.Model.Repositories
{
    public class AnswerRepository
    {
        private readonly NpgsqlConnection _connection;

        public AnswerRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }
        
        public bool CanAcceptAnswer(int answerId, int userId)
        {
            if (_connection == null)
            {
                throw new InvalidOperationException("Database connection is not initialized.");
            }
            
            _connection.Open();
            try
            {
                // using (var command = new NpgsqlCommand("SELECT COUNT(1) FROM questions WHERE id = (SELECT question_id FROM answer WHERE id = :answerId) AND questions.user_id = :userId", _connection))
                using (var command = new NpgsqlCommand("SELECT COUNT(1) FROM questions WHERE id = (SELECT question_id FROM answer WHERE id = :answerId) AND questions.user_id = :userId", _connection))
                {
                    command.Parameters.AddWithValue(":answerId", answerId);
                    command.Parameters.AddWithValue(":userId", userId);
                    var result = command.ExecuteScalar();
                    Console.WriteLine(result); // 0
                    bool canAccept = result != null;
                    return canAccept;
                }
            }
            finally
            {
                _connection.Close();
            }
        }

        public void AcceptAnswer(int answerId)
        {
            _connection.Open();
            using (var command = new NpgsqlCommand("UPDATE answer SET is_accepted = TRUE WHERE id = @answerId", _connection))
            {
                command.Parameters.AddWithValue("@answerId", answerId);
                command.ExecuteNonQuery();
                _connection.Close();
            }
        }

        public bool QuestionExists(int questionId)
        {
            _connection.Open();
            try
            {
                using (var command = new NpgsqlCommand("SELECT COUNT(1) FROM questions WHERE id = @questionId", _connection))
                {
                    command.Parameters.AddWithValue("@questionId", questionId);
                    var exists = (long)command.ExecuteScalar() > 0;
                    return exists;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error checking if question exists: {ex.Message}");
                throw;
            }
            finally
            {
                _connection.Close();
            }
        }

        public int Create(Answer answer)
        {
            if (!QuestionExists(answer.Question_id))
            {
                throw new ArgumentException("The question_id does not exist.");
            }

            _connection.Open();
            try
            {
                using (var command = new NpgsqlCommand(
                    "INSERT INTO answer (message, question_id, submission_time, user_id) VALUES (@message, @question_id, @submission_time, @user_id) RETURNING id", _connection))
                {
                    command.Parameters.AddWithValue("@message", answer.Message);
                    command.Parameters.AddWithValue("@question_id", answer.Question_id);
                    command.Parameters.AddWithValue("@submission_time", answer.Submission_time);
                    command.Parameters.AddWithValue("@user_id", answer.User_id);

                    var lastInsertId = (int)command.ExecuteScalar();
                    return lastInsertId;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error creating answer: {ex.Message}");
                throw;
            }
            finally
            {
                _connection.Close();
            }
        }

        public void Delete(int id)
        {
            _connection.Open();
            try
            {
                using (var command = new NpgsqlCommand("DELETE FROM answer WHERE id = @id", _connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error deleting answer: {ex.Message}");
                throw;
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}