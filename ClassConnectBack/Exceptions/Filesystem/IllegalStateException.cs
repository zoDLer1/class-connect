using Microsoft.AspNetCore.Mvc;

namespace ClassConnect.Exceptions;

public class IllegalStateException : ActionException<BadRequestObjectResult>
{
    public IllegalStateException(string message = "Недостаточно параметров")
        : base(message) { }
}
