using Microsoft.AspNetCore.Mvc;

namespace ClassConnect.Exceptions;

public interface IActionException<out T>
    where T : ActionResult
{
    public IActionResult FormResponse();
}
