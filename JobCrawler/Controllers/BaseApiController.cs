using JobCrawler.Domain.Results;
using Microsoft.AspNetCore.Mvc;

namespace JobScrawler.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class BaseApiController : Controller
{
    protected ActionResult HandleResult<T>(Result<T> result)
    {
        if (result == null)
            return NotFound();

        return result.IsSuccess switch
        {
            true when result.Value != null => Ok(result.Value),
            true when result.Value == null => NotFound(),
            false => BadRequest(new { message = result.Error }),
            _ => BadRequest()
        };
    }
}