using Microsoft.AspNetCore.Mvc;

namespace ClassConnect.Exceptions;

public class ActionException<T> : Exception, IActionException<T>
    where T : ActionResult
{
    public string? PropertyName { get; set; }

    public ActionException()
        : base() { }

    public ActionException(string? message)
        : base(message) { }

    public IActionResult FormResponse()
    {
        var key = "message";
        if (PropertyName != null)
            key = PropertyName;

        object errors = new Dictionary<string, List<string>> { [key] = new List<string> { Message } };
        var instance = Activator.CreateInstance(typeof(T), new { errors }) as T;
        if (instance == null)
            throw new InvalidCastException();
        return instance;
    }
}
