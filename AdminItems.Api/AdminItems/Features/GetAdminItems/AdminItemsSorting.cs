using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using SqlKata;

namespace AdminItems.Api.AdminItems.Features.GetAdminItems;

public class AdminItemsSorting
{
    private static readonly Dictionary<string, (string column, Func<Query, string, Query> orderBy)> _columns = new()
    {
        { "name", ("name", (q, o) => SortBy(q, "name", o)) },
        { "code", ("code", (q, o) => SortBy(q, "code", o)) },
        { "color", ("color", (q, o) => SortBy(q, "color", o)) }
    };

    private readonly string _column;
    private readonly string _order;
    private readonly Func<Query, string, Query> _orderBy;

    private AdminItemsSorting(string column, string order, Func<Query, string, Query> orderBy)
    {
        _column = column;
        _order = order;
        _orderBy = orderBy;
    }

    public static Result<AdminItemsSorting, BadRequestObjectResult> Parse(string orderBy)
    {
        var split = orderBy.Split(" ");
        if (split.Length != 2)
            return Result.Failure<AdminItemsSorting, BadRequestObjectResult>(
                ErrorResponses.InvalidOrderBy(orderBy, $"should be in format [column] [asc|desc]"));

        var column = split[0];
        var order = split[1];

        if (!_columns.ContainsKey(column))
        {
            return Result.Failure<AdminItemsSorting, BadRequestObjectResult>(
                ErrorResponses.InvalidOrderBy(orderBy, $"Column {column} was not found"));
        }

        if (!new[] { "asc", "desc" }.Contains(order))
            return Result.Failure<AdminItemsSorting, BadRequestObjectResult>(
                ErrorResponses.InvalidOrderBy(orderBy, $"Order {order} was not found"));

        var (name, orderer) = _columns[column];
        return Result.Success<AdminItemsSorting, BadRequestObjectResult>(new AdminItemsSorting(name, order, orderer));
    }

    public Query Apply(Query query) =>
        _orderBy.Invoke(query, _order)
            .Slice(column: _column);

    private static Query SortBy(Query query, string column, string order) =>
        order switch
        {
            "asc" => query.OrderBy(column),
            "desc" => query.OrderByDesc(column),
            _ => throw new ArgumentOutOfRangeException(nameof(order))
        };
}