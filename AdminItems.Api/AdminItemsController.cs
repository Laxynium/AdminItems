using Microsoft.AspNetCore.Mvc;

namespace AdminItems.Api;

public record AdminItemDto(string Code, string Name, string? Comments);

[ApiController]
[Route("[controller]")]
public class AdminItemsController : ControllerBase
{
    private readonly IAdminItemsStore _adminItemsStore;

    public AdminItemsController(IAdminItemsStore adminItemsStore)
    {
        _adminItemsStore = adminItemsStore;
    }
    
    [HttpPost]
    public Task Post([FromBody] AdminItemDto dto)
    {
        _adminItemsStore.Add(new AdminItem(
            dto.Code,
            dto.Name,
            dto.Comments ?? string.Empty));
        return Task.CompletedTask;
    }
}