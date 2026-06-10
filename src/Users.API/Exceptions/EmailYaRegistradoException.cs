namespace Users.API.Exceptions
{
    public class EmailYaRegistradoException : Exception
    {
        public string ErrorCode { get; } = "USR-001";
        public int StatusCode { get; } = 409;

        public EmailYaRegistradoException(string email)
            : base($"El email '{email}' ya está registrado.")
        {
        }
    }
}
