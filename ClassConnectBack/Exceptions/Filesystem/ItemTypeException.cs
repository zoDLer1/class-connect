using Microsoft.AspNetCore.Mvc;

namespace ClassConnect.Exceptions;

public class ItemTypeException : ActionException<BadRequestObjectResult>
{
    public ItemTypeException(string message = "Некорректный тип объекта")
        : base(message) { }
}
