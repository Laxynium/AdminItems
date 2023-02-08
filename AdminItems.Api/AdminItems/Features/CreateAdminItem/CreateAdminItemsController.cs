using System.ComponentModel.DataAnnotations;
using AdminItems.Api.Colors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminItems.Api.AdminItems.Features.CreateAdminItem;

public record AdminItemDto(
    [Required] [StringLength(12)] string Code,
    [Required] [StringLength(200)] string Name,
    long ColorId,
    string? Comments
);

[ApiController]
[Route("adminItems")]
[Authorize(Roles = "Admin-ManageItems")]
public class CreateAdminItemsController : ControllerBase
{
    private readonly IAdminItemsStore _adminItemsStore;
    private readonly IColorsStore _colorsStore;
    private readonly IAdminItemIdGenerator _adminItemIdGenerator;

    public CreateAdminItemsController(
        IAdminItemsStore adminItemsStore, 
        IColorsStore colorsStore, 
        IAdminItemIdGenerator adminItemIdGenerator)
    {
        _adminItemsStore = adminItemsStore;
        _colorsStore = colorsStore;
        _adminItemIdGenerator = adminItemIdGenerator;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] AdminItemDto dto)
    {
        var color = await _colorsStore.Find(dto.ColorId);
        if (color is null)
            return ErrorResponses.ColorNotFound(dto.ColorId);

        var id = _adminItemIdGenerator.NextId();
        
        await _adminItemsStore.Add(id, new AdminItem(
            dto.Code,
            dto.Name,
            dto.Comments ?? string.Empty,
            color.Name));
        
        return Created(string.Empty, new
        {
            id = id.Value
        });
    }
}