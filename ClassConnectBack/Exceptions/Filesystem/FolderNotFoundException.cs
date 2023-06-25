using Microsoft.AspNetCore.Mvc;

namespace ClassConnect.Exceptions;

public class FolderNotFoundException : ActionException<NotFoundObjectResult>
{
    public FolderNotFoundException(string message = "Папка не найдена")
        : base(message) { }
}
