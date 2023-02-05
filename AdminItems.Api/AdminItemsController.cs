using Microsoft.AspNetCore.Mvc;

namespace AdminItems.Api;

[ApiController]
[Route("[controller]")]
public class AdminItemsController : ControllerBase
{
    [HttpPost]
    public Task Post()
    {
        return Task.CompletedTask;
    }
}