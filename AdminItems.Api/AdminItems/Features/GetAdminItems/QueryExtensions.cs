using System.Diagnostics.Contracts;
using SqlKata;
using SqlKata.Compilers;

namespace AdminItems.Api.AdminItems.Features.GetAdminItems;

/// <summary>
/// Cursor based pagination taken from article
/// https://www.corstianboerman.com/blog/2019-03-06/cursor-based-pagination-with-sql-server
/// </summary>
internal static class QueryExtensions
{
    [Pure]
    public static Query Slice(
        this Query query,
        string after = null,
        int first = 0,
        string before = null,
        int last = 0,
        string column = "Id")
    {
        var queryClone = query.Clone();

        // Manually compile the order clauses for later use in the query
        var order = new SqlServerCompiler()
            .CompileOrders(new SqlResult
            {
                Query = queryClone
            });

        queryClone.Clauses.RemoveAll(q => q.Component == "order");

        if (string.IsNullOrWhiteSpace(order))
            throw new Exception($"{nameof(query)} does not have an order by clause");

        queryClone.SelectRaw($"ROW_NUMBER() OVER({order}) AS [RowNumber]");

        var internalQuery = new Query()
            .With("q", queryClone)
            .From("q");

        // Select all rows after provided cursor
        if (!String.IsNullOrWhiteSpace(after))
        {
            internalQuery.Where("RowNumber", ">",
                new Query("q")
                    .Select("RowNumber")
                    .Where(column, after));
        }

        // Select all rows before provided cursor
        if (!String.IsNullOrWhiteSpace(before))
        {
            internalQuery.Where("RowNumber", "<",
                new Query("q")
                    .Select("RowNumber")
                    .Where(column, before));
        }

        // Select the first x amount of rows
        if (first > 0)
        {
            // If the after cursor is defined
            if (!String.IsNullOrWhiteSpace(after))
            {
                internalQuery.Where("RowNumber", "<=",
                    new Query("q")
                        .SelectRaw($"[RowNumber] + {first}")
                        .Where(column, after));
            }
            // If no after cursor is defined
            else
            {
                internalQuery.Where("RowNumber", "<=", first);
            }
        }

        // Select the last x amount of rows
        if (last > 0)
        {
            // If the before cursor is defined
            if (!String.IsNullOrWhiteSpace(before))
            {
                internalQuery.Where("RowNumber", ">=",
                    new Query("q")
                        .SelectRaw($"[RowNumber] - {last}")
                        .Where(column, before));
            }
            // If we have to take data all the way from the back
            else
            {
                internalQuery.Where("RowNumber", ">",
                    new Query("q")
                        .SelectRaw($"MAX([RowNumber]) - {last}"));
            }
        }

        return internalQuery;
    }
}