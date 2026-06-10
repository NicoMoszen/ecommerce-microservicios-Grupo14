namespace Users.API.Exceptions
{
    public class CredencialesInvalidasException : Exception
    {
        public string ErrorCode { get; } = "USR-003";
        public int StatusCode { get; } = 401;

        public CredencialesInvalidasException()
            : base("Credenciales incorrectas.")
        {
        }
    }
}
