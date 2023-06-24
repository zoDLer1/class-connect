namespace ClassConnect.Models;

public class CommonModel<TId>
{
    public TId Id { get; set; } = default(TId)!;
}
