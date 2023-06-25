using Microsoft.AspNetCore.Mvc;

namespace ClassConnect.Exceptions;

public class InvalidPathException : ActionException<BadRequestObjectResult>
{
    public InvalidPathException(string message = "Передан неправильный путь")
        : base(message) { }
}
