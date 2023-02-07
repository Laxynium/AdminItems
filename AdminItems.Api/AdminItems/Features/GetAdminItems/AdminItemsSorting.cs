using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using SqlKata;

namespace AdminItems.Api.AdminItems.Features.GetAdminItems;

public class AdminItemsSorting
{
    public string Column { get; }
    public string Order { get; }

    private AdminItemsSorting(string column, string order)
    {
        Column = column;
        Order = order;
    }

    public static Result<AdminItemsSorting, BadRequestObjectResult> Parse(string orderBy)
    {
        var split = orderBy.Split(" ");
        if (split.Length != 2)
            return Result.Failure<AdminItemsSorting, BadRequestObjectResult>(
                ErrorResponses.InvalidOrderBy(orderBy, $"should be in format [column] [asc|desc]"));

        var column = split[0];
        var order = split[1];

        if (!new[] { "code", "name", "color" }.Contains(column))
            return Result.Failure<AdminItemsSorting, BadRequestObjectResult>(
                ErrorResponses.InvalidOrderBy(orderBy, $"Column {column} was not found"));

        if(!new []{"asc", "desc"}.Contains(order))
            return Result.Failure<AdminItemsSorting, BadRequestObjectResult>(
                ErrorResponses.InvalidOrderBy(orderBy, $"Order {order} was not found"));

        return Result.Success<AdminItemsSorting, BadRequestObjectResult>(new AdminItemsSorting(column, order));
    }

    public Query Apply(Query query)
    {
        switch (Column)
        {
            case "code":
                query.OrderBy("code");
                break;
            case "name":
                query.OrderBy("name");
                break;
            case "color":
                query.OrderBy("color");
                break;
            default:
                throw new ArgumentOutOfRangeException("column");
        }
        return query.Slice(column: Column);
    }
}