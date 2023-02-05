﻿using System.ComponentModel.DataAnnotations;
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

    public UpdateAdminItemController(IAdminItemsStore adminItemsStore)
    {
        _adminItemsStore = adminItemsStore;
    }
    [HttpPut("{adminItemId}")]
    public async Task Put([FromRoute]long adminItemId, [FromBody] AdminItemDto dto)
    {
        await _adminItemsStore.Update(1, new AdminItem(dto.Code, dto.Name, dto.Comments!, "indigo"));
    }
}