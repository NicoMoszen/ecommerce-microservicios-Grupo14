using Microsoft.AspNetCore.Mvc;
using Users.API.DTOs;
using Users.API.Services;

namespace Users.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController()
        {
            _userService = new UserService();
        }

        [HttpPost("register")]
        public IActionResult Registrar([FromBody] RegisterRequest request)
        {
            var resultado = _userService.Registrar(request);
            return StatusCode(201, resultado);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var resultado = _userService.Login(request);
            return Ok(resultado);
        }
    }
}
