using Microsoft.AspNetCore.Mvc;

namespace ClassConnect.Exceptions;

public class ItemNotFoundException : ActionException<NotFoundObjectResult>
{
    public ItemNotFoundException(string message = "Объект не найден")
        : base(message) { }
}
