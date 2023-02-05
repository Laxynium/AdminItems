using Microsoft.AspNetCore.Mvc;

namespace AdminItems.Api.AdminItems.GetAdminItems;

public record Response(IReadOnlyList<AdminItemResponse> Items);
public record AdminItemResponse(string Code, string Name, string Color, string? Comments);
    
[ApiController]
[Route("adminItems")]
public class GetAdminItemsController : ControllerBase
{
    private readonly IAdminItemsStore _adminItemsStore;

    public GetAdminItemsController(IAdminItemsStore adminItemsStore)
    {
        _adminItemsStore = adminItemsStore;
    }
    [HttpGet]
    public async Task<ActionResult<Response>> Get()
    {
        var items = await _adminItemsStore.GetAll(
            MapToResponse,
            x => x.Code);
        return Ok(new Response(items));
    }

    private static AdminItemResponse MapToResponse(AdminItem x)
    {
        return new AdminItemResponse(x.Code, x.Name, x.Color, x.Comments);
    }
}