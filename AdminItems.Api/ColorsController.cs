using Microsoft.AspNetCore.Mvc;

namespace AdminItems.Api;

[ApiController]
[Route("[controller]")]
public class ColorsController : ControllerBase
{

    [HttpGet]
    public Task Get()
    {
        return Task.CompletedTask;
    }
}