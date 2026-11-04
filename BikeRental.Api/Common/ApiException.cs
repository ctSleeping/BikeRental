namespace BikeRental.Api.Common;

// Base class for expected errors. Carries the HTTP status code
// and a machine-readable error code.
public abstract class ApiException : Exception
{
    public int StatusCode { get; }
    public string Code { get; }

    protected ApiException(int statusCode, string code, string message)
        : base(message)
    {
        StatusCode = statusCode;
        Code = code;
    }
}

public class UnauthorizedApiException : ApiException
{
    public UnauthorizedApiException(string code, string message)
        : base(401, code, message) { }
}

public class NotFoundApiException : ApiException
{
    public NotFoundApiException(string code, string message)
        : base(404, code, message) { }
}

public class ConflictApiException : ApiException
{
    public ConflictApiException(string code, string message)
        : base(409, code, message) { }
}