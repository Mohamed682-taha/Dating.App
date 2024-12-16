using Dating.API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Dating.API.Controllers;

[ServiceFilter(typeof(LogUserActivity))]
[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
}