using System.ComponentModel.DataAnnotations;
using AdminItems.Api.Colors;
using Microsoft.AspNetCore.Mvc;

namespace AdminItems.Api.AdminItems.UpdateAdminItem;

public record AdminItemDto(
    [Required] [StringLength(12)] string Code,
    [Required] [StringLength(200)] string Name,
    long ColorId,
    string? Comments
);

[ApiController]
[Route("adminItems")]
public class UpdateAdminItemController : ControllerBase
{
    private readonly IAdminItemsStore _adminItemsStore;
    private readonly IColorsStore _colorsStore;

    public UpdateAdminItemController(IAdminItemsStore adminItemsStore, IColorsStore colorsStore)
    {
        _adminItemsStore = adminItemsStore;
        _colorsStore = colorsStore;
    }
    
    [HttpPut("{adminItemId}")]
    public async Task<ActionResult> Put([FromRoute]long adminItemId, [FromBody] AdminItemDto dto)
    {
        var color = await _colorsStore.Find(dto.ColorId);
        if (color is null)
        {
            return ColorNotFound(dto.ColorId);
        }
        var adminItem = new AdminItem(dto.Code, dto.Name, dto.Comments ?? string.Empty, color!.Name);
        await _adminItemsStore.Update(AdminItemId.Create(adminItemId), adminItem);
        return Ok();
    }
    
    private BadRequestObjectResult ColorNotFound(long colorId) =>
        BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
        {
            {nameof(colorId), new []{$"Color with id {colorId} was not found"}}
        }));

}