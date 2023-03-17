namespace PracticeWeb.Models;

public class Work : StringIdCommonModel
{
    public bool IsSubmitted { get; set; } = false;
    public DateTime? SubmitDate { get; set; }
    public int? Mark { get; set; }
}
