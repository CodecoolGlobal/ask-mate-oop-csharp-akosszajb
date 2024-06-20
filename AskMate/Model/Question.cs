namespace AskMate.Model;

public class Question
{

 public int Id { get; set; }
 public string Title { get; set; }
 public string Description { get; set; }
 public DateTime Submission_time { get; set; }
 public int User_id { get; set; }

}
