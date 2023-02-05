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
    public Task Post([FromBody] AdminItemDto dto)
    {
        var colors = _colorsStore.GetAll(x => x);
        var color = colors.FirstOrDefault(x => x.Id == dto.ColorId);
        _adminItemsStore.Add(new AdminItem(
            dto.Code,
            dto.Name,
            dto.Comments ?? string.Empty,
            color?.Name ?? "indigo"));
        return Task.CompletedTask;
    }
}