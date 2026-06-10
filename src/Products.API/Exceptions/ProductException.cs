namespace Products.API.Exceptions
{
    public class ProductException : Exception
    {
        public string ErrorCode { get; }
        public int StatusCode { get; }

        public ProductException(string errorCode, string message, int statusCode)
            : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }
    }
}