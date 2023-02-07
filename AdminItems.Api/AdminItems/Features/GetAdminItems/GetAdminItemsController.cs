using Dapper;
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
        var sqlBuilder = new SqlBuilder();
        sqlBuilder = SortBy(sqlBuilder, "code");
        
        var template = GetTemplate(sqlBuilder);
        var result = await _connection.QueryAsync<AdminItemResponse>(
            template.RawSql, template.Parameters);
        return Ok(new Response(result.ToList()));
    }

    private static SqlBuilder.Template GetTemplate(SqlBuilder builder) =>
        new(builder,@"
SELECT ""id""
    , ""code""
    , ""name""
    , ""color""
FROM ""admin_items""
/**orderby**/", new { });

    private static SqlBuilder SortBy(SqlBuilder sqlBuilder, string fieldName)
    {
        switch (fieldName)
        {
            case "code":
                sqlBuilder.OrderBy("code");
                break;
        }

        return sqlBuilder;
    }
}