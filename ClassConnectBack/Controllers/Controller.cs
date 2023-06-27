using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ClassConnect.Exceptions;
using ClassConnect.Models;
using ClassConnect.Services.UserServices;

namespace ClassConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Controller : ControllerBase
    {
        private IUserService _userService;

        public Controller(IUserService userService)
        {
            _userService = userService;
        }

        protected async Task<User> GetUserAsync()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
                throw new UserNotFoundException();

            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
                throw new UserNotFoundException();

            if (!user.IsActivated)
                throw new AccessDeniedException();

            return user;
        }
    }
}
