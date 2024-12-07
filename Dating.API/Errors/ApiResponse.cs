namespace Dating.API.Errors;

public class ApiResponse(int statusCode, string? message = null)
{
    public int StatusCode { get; set; } = statusCode;
    public string? Message { get; set; } = message ?? GetDefaultMessageForStatusCode(statusCode);

    private static string? GetDefaultMessageForStatusCode(int statusCode)
    {
        return statusCode switch
        {
            400 => "You Have Made a Bad Request",
            401 => "Your Are Not Authorized",
            404 => "Resource Was Not Found",
            500 => "Internal Server Error",
            _ => null
        };
    }
}