using Microsoft.AspNetCore.Mvc;

namespace ClassConnect.Exceptions;

public class InvalidUserRoleException : ActionException<BadRequestObjectResult>
{
    public InvalidUserRoleException(string message = "Пользователь имеет недостаточно прав")
        : base(message) { }
}
