using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace AdminItems.Api.AdminItems.Features.GetAdminItems;

public record Request(string OrderBy = "code asc");
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
    public async Task<ActionResult<Response>> Get([FromQuery] Request request)
    {
        var (_, isFailure, sorting, error) = AdminItemsSorting.Parse(request.OrderBy);
        if (isFailure)
            return error;

        var query = sorting.Apply(new Query("admin_items").Select("id", "code", "name", "color"));
        
        var postgresCompiler = new PostgresCompiler();
        var db = new QueryFactory(_connection, postgresCompiler);

        var queryResult = await db.FromQuery(query).GetAsync<AdminItemRecord>();

        var items = queryResult
            .Select(x => new AdminItemResponse(x.Id, x.Code, x.Name, x.Color))
            .ToList();
        
        return Ok(new Response(items));
    }
    private record AdminItemRecord(long Id, string Code, string Name, string Color, long RowNumber);
}