﻿using Microsoft.AspNetCore.Mvc;
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
        var ordering = ParseOrderBy(request.OrderBy);

        if (ordering is not {ordering:{order:{} order, column:{} column}})
            return ordering.error!;

        var query = new Query("admin_items")
            .Select("id", "code", "name", "color");

        query = SortBy(query, column);
        query = query.Slice(column: column);
        
        var postgresCompiler = new PostgresCompiler();
        var db = new QueryFactory(_connection, postgresCompiler);

        var queryResult = await db.FromQuery(query).GetAsync<AdminItemRecord>();

        var items = queryResult
            .Select(x => new AdminItemResponse(x.Id, x.Code, x.Name, x.Color))
            .ToList();
        
        return Ok(new Response(items));
    }

    private static ((string column, string order)? ordering, BadRequestObjectResult? error) ParseOrderBy(string orderBy)
    {
        var split = orderBy.Split(" ");
        var column = split[0];
        var order = split[1];

        return ((column, order), null);
    }
    private record AdminItemRecord(long Id, string Code, string Name, string Color, long RowNumber);
    private static Query SortBy(Query query, string fieldName)
    {
        switch (fieldName)
        {
            case "code":
                query.OrderBy("code");
                break;
        }

        return query;
    }
}