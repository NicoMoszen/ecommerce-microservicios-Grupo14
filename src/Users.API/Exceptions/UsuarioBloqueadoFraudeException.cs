namespace Users.API.Exceptions
{
    public class UsuarioBloqueadoFraudeException : Exception
    {
        public string ErrorCode { get; } = "USR-005";
        public int StatusCode { get; } = 403;

        public UsuarioBloqueadoFraudeException()
            : base("Su cuenta fue suspendida por razones de seguridad. Contacte a soporte.")
        {
        }
    }
}
