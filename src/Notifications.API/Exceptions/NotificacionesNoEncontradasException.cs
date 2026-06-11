namespace Notifications.API.Exceptions
{
    public class NotificacionesNoEncontradasException : Exception
    {
        public string ErrorCode { get; } = "NTF-003";
        public int StatusCode { get; } = 404;

        public NotificacionesNoEncontradasException(Guid userId)
            : base($"No se encontraron notificaciones para el usuario.")
        {
        }
    }
}
