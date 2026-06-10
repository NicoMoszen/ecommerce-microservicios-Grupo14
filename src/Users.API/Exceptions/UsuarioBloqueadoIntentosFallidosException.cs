namespace Users.API.Exceptions
{
    public class UsuarioBloqueadoIntentosFallidosException : Exception
    {
        public string ErrorCode { get; } = "USR-004";
        public int StatusCode { get; } = 403;

        public UsuarioBloqueadoIntentosFallidosException()
            : base("Su cuenta fue bloqueada por superar el máximo de intentos fallidos. Contacte a soporte.")
        {
        }
    }
}
