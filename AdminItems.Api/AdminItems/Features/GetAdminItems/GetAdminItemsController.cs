using System.ComponentModel.DataAnnotations;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace AdminItems.Api.AdminItems.Features.GetAdminItems;

public record Request([Range(1, 200)]int PageSize = 25, string? Before = null, string? After = null, string OrderBy = "code asc");
public record Response(IReadOnlyList<AdminItemResponse> Items, string? Before = null, string? After = null);
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

        var query = sorting.ApplySorting(new Query("admin_items").Select("id", "code", "name", "color"));
        query = sorting.ApplyPagination(query, request.PageSize, request.After);
        
        var postgresCompiler = new PostgresCompiler();
        var db = new QueryFactory(_connection, postgresCompiler);

        var queryResult = (await db.FromQuery(query).GetAsync<AdminItemRecord>()).ToList();
        
        var items = queryResult
            .Select(x => new AdminItemResponse(x.Id, x.Code, x.Name, x.Color))
            .ToList();

        var (before, after) = sorting.GetSlice(queryResult);
        
        return Ok(new Response(items, before, after));
    }
}