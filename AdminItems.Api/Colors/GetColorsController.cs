using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace AdminItems.Api.Colors;

public record Response(IReadOnlyList<ColorDto> Items);

public record ColorDto(long Id, string Name);

[ApiController]
[Route("colors")]
[Authorize]
public class GetColorsController : ControllerBase
{
    private readonly NpgsqlConnection _connection;

    public GetColorsController(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    [HttpGet]
    public async Task<Response> Get()
    {
        var result = await _connection.QueryAsync<ColorDto>(@"
SELECT ""id""
     , ""name""
FROM colors c");

        return new Response(result.ToList());
    }
}