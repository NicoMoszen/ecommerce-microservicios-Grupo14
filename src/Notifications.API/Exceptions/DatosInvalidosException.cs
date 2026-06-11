namespace Notifications.API.Exceptions
{
    public class DatosInvalidosException : Exception
    {
        public string ErrorCode { get; } = "NTF-002";
        public int StatusCode { get; } = 400;

        public DatosInvalidosException(string message) : base(message)
        {
        }
    }
}
