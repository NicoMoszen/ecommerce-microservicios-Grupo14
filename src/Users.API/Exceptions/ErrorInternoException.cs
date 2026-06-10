namespace Users.API.Exceptions
{
    public class ErrorInternoException : Exception
    {
        public string ErrorCode { get; } = "USR-006";
        public int StatusCode { get; } = 500;

        public ErrorInternoException(string message)
            : base(message)
        {
        }
    }
}
