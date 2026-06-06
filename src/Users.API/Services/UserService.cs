using Users.API.DTOs;
using Users.API.Exceptions;
using Users.API.Models;

namespace Users.API.Services
{
    public class UserService
    {
        // NICO: reemplazar por conexión a base de datos
        private static List<User> _usuarios = new List<User>();

        public UserResponse Registrar(RegisterRequest request)
        {
            ValidarRegistro(request);
            var usuarioExistente = _usuarios.FirstOrDefault(u => u.Email == request.Email);
            if (usuarioExistente != null)
                throw new EmailYaRegistradoException(request.Email);

            var nuevoUsuario = new User
            {
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Email = request.Email,
                PasswordHash = request.Password
            };

            _usuarios.Add(nuevoUsuario);

            return new UserResponse
            {
                Id = nuevoUsuario.Id,
                Nombre = nuevoUsuario.Nombre,
                Apellido = nuevoUsuario.Apellido,
                Email = nuevoUsuario.Email,
                Activo = nuevoUsuario.Activo,
                FechaRegistro = nuevoUsuario.FechaRegistro
            };
        }

        public UserResponse Login(LoginRequest request)
        {
            var usuario = _usuarios.FirstOrDefault(u => u.Email == request.Email);
            if (usuario == null)
                throw new CredencialesInvalidasException();

            if (!usuario.Activo)
                throw new UsuarioBloqueadoIntentosFallidosException();

            if (usuario.PasswordHash != request.Password)
            {
                usuario.IntentosFallidos++;
                if (usuario.IntentosFallidos >= 3)
                {
                    usuario.Activo = false;
                    throw new UsuarioBloqueadoIntentosFallidosException();
                }
                throw new CredencialesInvalidasException();
            }

            usuario.IntentosFallidos = 0;

            return new UserResponse
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Activo = usuario.Activo,
                FechaRegistro = usuario.FechaRegistro
            };
        }
        private void ValidarRegistro(RegisterRequest request)
        {
            List<string> errores = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Nombre))
                errores.Add("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Apellido))
                errores.Add("El apellido es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Email))
                errores.Add("El email es obligatorio.");
            else if (!request.Email.Contains("@") || !request.Email.Contains("."))
                errores.Add("El email no tiene un formato válido.");

            if (string.IsNullOrWhiteSpace(request.Password))
                errores.Add("La contraseña es obligatoria.");
            else if (request.Password.Length < 8)
                errores.Add("La contraseña debe tener al menos 8 caracteres.");

            if (errores.Count > 0)
            {
                string mensaje = string.Join(" ", errores);
                throw new DatosInvalidosException(mensaje);
            }
        }
    }
}
