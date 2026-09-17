namespace PurchaseBillManagement.Api.Services.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; }

        public ApiException(int statusCode, string message, Exception? innerException = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
