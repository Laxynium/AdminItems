using System.ComponentModel.DataAnnotations;
using AdminItems.Api.Colors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminItems.Api.AdminItems.Features.UpdateAdminItem;

public record AdminItemDto(
    [Required] [StringLength(12)] string Code,
    [Required] [StringLength(200)] string Name,
    long ColorId,
    string? Comments
);

[ApiController]
[Route("adminItems")]
[Authorize(Roles = "Admin-ManageItems")]
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
        var id = AdminItemId.Create(adminItemId);
        if (!await _adminItemsStore.Contains(id))
        {
            return ErrorResponses.AdminItemNotFound(adminItemId);
        }
        
        var color = await _colorsStore.Find(dto.ColorId);
        if (color is null)
            return ErrorResponses.ColorNotFound(dto.ColorId);
        
        var adminItem = new AdminItem(dto.Code, dto.Name, dto.Comments ?? string.Empty, color!.Name);
        await _adminItemsStore.Update(id, adminItem);
        return Ok();
    }
}