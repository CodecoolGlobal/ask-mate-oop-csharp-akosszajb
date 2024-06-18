namespace AskMate.Model;

public class Answer
{

    public int Id { get; set; }
    public string Message { get; set; }
    public int Question_id { get; set; }
    public DateTime Submission_time { get; set; }

}