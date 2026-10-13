namespace BikeRental.Api.Common;

// Base class for all "expected" errors from the business logic.
// Carries the HTTP status code and the machine-readable code
// that the frontend reads (e.g. "BikeNotFound").
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

// 404 – something doesn't exist (BikeNotFound, RentalNotFound...)
public class NotFoundApiException : ApiException
{
    public NotFoundApiException(string code, string message)
        : base(404, code, message) { }
}

// 409 – the request conflicts with a rule (BikeNotAvailable, BikeRented...)
public class ConflictApiException : ApiException
{
    public ConflictApiException(string code, string message)
        : base(409, code, message) { }
}