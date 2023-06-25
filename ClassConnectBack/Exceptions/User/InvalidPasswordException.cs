using Microsoft.AspNetCore.Mvc;

namespace ClassConnect.Exceptions;

public class InvalidPasswordException : ActionException<BadRequestObjectResult>
{
    public InvalidPasswordException(string message = "Неправильный пароль")
        : base(message) { }
}
