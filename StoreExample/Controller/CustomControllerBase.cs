using Microsoft.AspNetCore.Mvc;
using StoreExample.ErrorHandling;

namespace StoreExample.Controller;

[ApiController]
[Route("[controller]")]
public class CustomControllerBase : ControllerBase
{
    protected IActionResult Failure<T>(ErrorOr<T> error)
    {
        return StatusCode(error.StatusCode, new { Errors = error.Errors });
    }
}