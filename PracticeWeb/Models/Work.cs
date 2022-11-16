namespace PracticeWeb.Models;

public class Work : StringIdCommonModel
{
    public string SubjectId { get; set; } = null!;
    public Subject Subject { get; set; } = null!;
    
    public int StudentId { get; set; }
    public User Student { get; set; } = null!;
    
    public bool IsSubmitted { get; set; }
    public int? Mark { get; set; }
}