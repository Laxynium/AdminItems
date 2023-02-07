﻿using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace AdminItems.Api.AdminItems.Features.GetAdminItems;

public record Response(IReadOnlyList<AdminItemResponse> Items);
public record AdminItemResponse(long Id, string Code, string Name, string Color);
    
[ApiController]
[Route("adminItems")]
public class GetAdminItemsController : ControllerBase
{
    private readonly NpgsqlConnection _connection;

    public GetAdminItemsController(NpgsqlConnection connection)
    {
        _connection = connection;
    }
    
    [HttpGet]
    public async Task<ActionResult<Response>> Get()
    {
        var result = await _connection.QueryAsync<AdminItemResponse>(@"
SELECT ""id""
    , ""code""
    , ""name""
    , ""color""
FROM ""admin_items""
ORDER BY ""code""");
        return Ok(new Response(result.ToList()));
    }

    private static AdminItemResponse MapToResponse(AdminItemId id, AdminItem x) =>
        new(id, x.Code, x.Name, x.Color, x.Comments);
}