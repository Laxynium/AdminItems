using Microsoft.AspNetCore.Mvc;

namespace AdminItems.Api.AdminItems.GetAdminItems;

public record Response(IReadOnlyList<AdminItemResponse> Items);
public record AdminItemResponse(string Code, string Name, string Color, string? Comments);
    
[ApiController]
[Route("adminItems")]
public class GetAdminItemsController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Response>> Get()
    {
        return Ok(new Response(new List<AdminItemResponse>
        {
            new(
                "ADBD123",
                "Some random admin item name",
                "indigo",
                ""
            )
        }));
    }
}