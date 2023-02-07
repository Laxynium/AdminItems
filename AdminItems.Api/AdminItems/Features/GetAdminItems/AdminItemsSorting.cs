using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using SqlKata;

namespace AdminItems.Api.AdminItems.Features.GetAdminItems;

public class AdminItemsSorting
{
    private static readonly Dictionary<string, (string column, Func<Query, string, Query> orderBy, Func<AdminItemRecord, string?> selectValue)> _columns = new()
    {
        { "name", ("name", (q, o) => SortBy(q, "name", o), x => x.Name) },
        { "code", ("code", (q, o) => SortBy(q, "code", o), x => x.Code) },
        { "color", ("color", (q, o) => SortBy(q, "color", o), x=> x.Color) }
    };

    private readonly string _column;
    private readonly string _order;
    private readonly Func<Query, string, Query> _orderBy;
    private readonly Func<AdminItemRecord, string> _selectValue;

    private AdminItemsSorting(string column, string order, Func<Query, string, Query> orderBy, Func<AdminItemRecord, string> selectValue)
    {
        _column = column;
        _order = order;
        _orderBy = orderBy;
        _selectValue = selectValue;
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

        var (name, orderer, selectValue) = _columns[column];
        return Result.Success<AdminItemsSorting, BadRequestObjectResult>(new AdminItemsSorting(name, order, orderer, selectValue));
    }

    public Query ApplySorting(Query query) =>
        _orderBy.Invoke(query, _order);

    public Query ApplyPagination(Query query, int pageSize, string? after) =>
        query.Slice(after: after, first: pageSize, column: _column);

    internal (string? before, string? after) GetSlice(IReadOnlyCollection<AdminItemRecord> items)
    {
        var first = items.FirstOrDefault();
        var last = items.LastOrDefault();

        return (
            first is not null ? _selectValue(first) : null,
            last is not null ? _selectValue(last) : null
        );
    }
    
    private static Query SortBy(Query query, string column, string order) =>
        order switch
        {
            "asc" => query.OrderBy(column),
            "desc" => query.OrderByDesc(column),
            _ => throw new ArgumentOutOfRangeException(nameof(order))
        };
}