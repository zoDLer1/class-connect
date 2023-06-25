using Microsoft.AspNetCore.Mvc;

namespace ClassConnect.Exceptions;

public class InvalidNameException : ActionException<BadRequestObjectResult>
{
    public InvalidNameException(string message = "Некорректное название")
        : base(message) { }
}
