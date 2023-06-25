using Microsoft.AspNetCore.Mvc;

namespace ClassConnect.Exceptions;

public class UserNotFoundException : ActionException<NotFoundObjectResult>
{
    public UserNotFoundException(string message = "Пользователь не найден")
        : base(message) { }
}
