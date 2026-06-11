namespace Users.API.Exceptions
{
    public class NotFoundException : Exception
    {
        public string ErrorCode { get; } = "USR-007";
        public int StatusCode { get; } = 404;

        public NotFoundException(string message) : base(message)
        {
        }
    }
}