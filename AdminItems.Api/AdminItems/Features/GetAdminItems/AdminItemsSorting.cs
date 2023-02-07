using System.Globalization;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using SqlKata;

namespace AdminItems.Api.AdminItems.Features.GetAdminItems;

public class AdminItemsSorting
{
    private static readonly Dictionary<string, (string[] columns, Func<AdminItemRecord, string[]>)> ColumnsDefinitions = new()
    {
        { "name", (new[] { "name" }, x => new[]{x.Name}) },
        { "code", (new[] { "code" }, x => new[]{x.Code}) },
        { "color", (new[] { "color", "id" }, x => new[]{x.Color, x.Id.ToString(CultureInfo.InvariantCulture)}) }
    };

    private readonly string[] _columns;
    private readonly string _order;
    
    private readonly Func<AdminItemRecord, string[]> _selectFields;

    private AdminItemsSorting(string[] columns, string order, Func<AdminItemRecord, string[]> selectFields)
    {
        _columns = columns;
        _order = order;
        _selectFields = selectFields;
    }

    public static Result<AdminItemsSorting, BadRequestObjectResult> Parse(string orderBy)
    {
        var split = orderBy.Split(" ");
        if (split.Length != 2)
            return Result.Failure<AdminItemsSorting, BadRequestObjectResult>(
                ErrorResponses.InvalidOrderBy(orderBy, $"should be in format [column] [asc|desc]"));

        var column = split[0];
        var order = split[1];

        if (!ColumnsDefinitions.ContainsKey(column))
        {
            return Result.Failure<AdminItemsSorting, BadRequestObjectResult>(
                ErrorResponses.InvalidOrderBy(orderBy, $"Column {column} was not found"));
        }

        if (!new[] { "asc", "desc" }.Contains(order))
            return Result.Failure<AdminItemsSorting, BadRequestObjectResult>(
                ErrorResponses.InvalidOrderBy(orderBy, $"Order {order} was not found"));

        var (columns, getFields) = ColumnsDefinitions[column];
        
        return Result.Success<AdminItemsSorting, BadRequestObjectResult>(new AdminItemsSorting(columns, order, getFields));
    }

    public Query BuildQuery(int pageSize, string[]? before, string[]? after)
    {
        var innerQuery = new Query("admin_items").Select("id", "code", "name", "color");
        ApplyPaginationOn(innerQuery, _columns, pageSize, before, after).As("inner");

        var query = new Query().From(innerQuery);

        query = _order switch
        {
            "asc" => query.OrderBy(_columns),
            "desc" => query.OrderByDesc(_columns),
            _ => query
        };

        return query;
    }
    
    internal (string[]? before, string[]? after) GetSlice(IReadOnlyCollection<AdminItemRecord> items)
    {
        var first = items.FirstOrDefault();
        var last = items.LastOrDefault();

        return (
            first is not null ? _selectFields(first) : null,
            last is not null ? _selectFields(last) : null
        );
    }
    
    private  Query ApplyPaginationOn(Query query, string[] columns, int pageSize, string[]? before, string[]? after)
    {
        var (op, values) = GetValues(_order, before, after);
        
        query = values.Length switch
        {
            > 1 => query.WhereRaw($"({string.Join(",", columns)}) {op} ({string.Join(",", values.Select(_ =>"?"))})", values),
            1 => query.Where(columns[0], op, values[0]),
            _ => query
        };

        query = op switch
        {
            ">" => query.OrderBy(columns),
            "<" => query.OrderByDesc(columns),
            _ => _order switch
            {
                "asc" => query.OrderBy(_columns),
                "desc" => query.OrderByDesc(_columns),
                _ => query
            }
        };

        query.Limit(pageSize);

        return query;
    }
    
    private static (string op, object[] values) GetValues(string order, string[]? before, string[]? after)
    {
        var op = "none";
        var values = Array.Empty<object>();
        
        if (after is not null && after.Length > 0)
        {
            op = ">";
            values = after.Select(ToObject).ToArray();
        }

        if (before is not null && before.Length > 0)
        {
            op = "<";
            values = before.Select(ToObject).ToArray();
        }

        op = (order, op) switch
        {
            ("asc", ">") => ">",
            ("asc", "<") => "<",
            ("desc", ">") => "<",
            ("desc", "<") => ">",
            _ => op
        };
        return (op, values);
    }

    private static object ToObject(string value)
    {
        if (long.TryParse(value, out var number))
        {
            return number;
        }

        return value;
    }
}