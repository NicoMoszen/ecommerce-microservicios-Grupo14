namespace Users.API.Exceptions
{
    public class DatosInvalidosException : Exception
    {
        public string ErrorCode { get; } = "USR-002";
        public int StatusCode { get; } = 400;

        public DatosInvalidosException(string message)
            : base(message)
        {
        }
    }
}
