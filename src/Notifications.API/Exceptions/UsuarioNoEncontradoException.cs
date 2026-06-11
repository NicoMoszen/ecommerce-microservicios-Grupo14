namespace Notifications.API.Exceptions
{
    public class UsuarioNoEncontradoException : Exception
    {
        public string ErrorCode { get; } = "NTF-001";
        public int StatusCode { get; } = 404;

        public UsuarioNoEncontradoException(Guid usuarioId)
            : base($"El usuario destinatario no fue encontrado.")
        {
        }
    }
}
