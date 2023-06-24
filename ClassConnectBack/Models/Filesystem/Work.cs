namespace ClassConnect.Models;

public class Work : CommonModel<string>
{
    public bool IsSubmitted { get; set; } = false;
    public DateTime? SubmitDate { get; set; }
    public int? Mark { get; set; }
    public Item Item { get; set; } = null!;
}
