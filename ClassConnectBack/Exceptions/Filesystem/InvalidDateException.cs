using Microsoft.AspNetCore.Mvc;

namespace ClassConnect.Exceptions;

public class InvalidDateException : ActionException<BadRequestObjectResult>
{
    public InvalidDateException(string message = "Некорректная дата")
        : base(message) { }
}
