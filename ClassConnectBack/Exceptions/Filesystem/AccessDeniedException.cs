using Microsoft.AspNetCore.Mvc;

namespace ClassConnect.Exceptions;

public class AccessDeniedException : ActionException<ForbidResult> { }
