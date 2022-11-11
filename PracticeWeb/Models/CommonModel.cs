namespace PracticeWeb.Models;

public interface CommonModel<TId> 
{
    public TId Id { get; set; }
}