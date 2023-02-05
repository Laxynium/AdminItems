using System.ComponentModel.DataAnnotations;
using AdminItems.Api.Colors;
using Microsoft.AspNetCore.Mvc;

namespace AdminItems.Api.AdminItems;

public record AdminItemDto(
    [Required] [StringLength(12)] string Code,
    [Required] [StringLength(200)] string Name,
    long ColorId,
    string? Comments
);

[ApiController]
[Route("[controller]")]
public class AdminItemsController : ControllerBase
{
    private readonly IAdminItemsStore _adminItemsStore;
    private readonly IColorsStore _colorsStore;

    public AdminItemsController(IAdminItemsStore adminItemsStore, IColorsStore colorsStore)
    {
        _adminItemsStore = adminItemsStore;
        _colorsStore = colorsStore;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] AdminItemDto dto)
    {
        var color = await _colorsStore.Find(dto.ColorId);
        if (color is null)
            return ColorNotFound(dto.ColorId);

        await _adminItemsStore.Add(new AdminItem(
            dto.Code,
            dto.Name,
            dto.Comments ?? string.Empty,
            color.Name));
        
        return Ok();
    }

    private BadRequestObjectResult ColorNotFound(long colorId) =>
        BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
        {
            {nameof(colorId), new []{$"Color with id {colorId} was not found"}}
        }));
}