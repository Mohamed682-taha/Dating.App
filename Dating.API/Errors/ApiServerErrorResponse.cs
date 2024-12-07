namespace Dating.API.Errors;

public class ApiServerErrorResponse(int statusCode, string? message = null, string? details = null)
    : ApiResponse(statusCode, message)
{
    public string? Details { get; set; } = details;
}