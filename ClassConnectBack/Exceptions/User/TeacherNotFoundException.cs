using Microsoft.AspNetCore.Mvc;

namespace ClassConnect.Exceptions;

public class TeacherNotFoundException : ActionException<NotFoundObjectResult>
{
    public TeacherNotFoundException(string message = "Преподаватель не найден")
        : base(message) { }
}
