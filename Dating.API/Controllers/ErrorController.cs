using Dating.API.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Dating.API.Controllers;

[Route("errors/{code:int}")]
[ApiController]
public class ErrorController : Controller
{
    public ActionResult Error(int code)
    {
        return code switch
        {
            401 => Unauthorized(new ApiResponse(401)),
            404 => NotFound(new ApiResponse(404)),
            _ => StatusCode(code)
        };
    }
}