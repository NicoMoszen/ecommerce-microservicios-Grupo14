namespace Notifications.API.Exceptions
{
    public class ErrorInternoException : Exception
    {
        public string ErrorCode { get; } = "NTF-004";
        public int StatusCode { get; } = 500;

        public ErrorInternoException(string message) : base(message)
        {
        }
    }
}
