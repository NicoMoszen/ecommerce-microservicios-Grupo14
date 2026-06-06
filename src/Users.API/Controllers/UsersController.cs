using Microsoft.AspNetCore.Mvc;
using Users.API.DTOs;
using Users.API.Services;

namespace Users.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController()
        {
            _userService = new UserService();
        }

        /// <summary>
        /// Registrar un nuevo usuario
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(UserResponse), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public IActionResult Registrar([FromBody] RegisterRequest request)
        {
            var resultado = _userService.Registrar(request);
            return StatusCode(201, resultado);
        }

        /// <summary>
        /// Autenticar un usuario con email y contraseña
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(UserResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var resultado = _userService.Login(request);
            return Ok(resultado);
        }
    }
}