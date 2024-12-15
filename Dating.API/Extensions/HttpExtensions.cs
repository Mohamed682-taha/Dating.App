using System.Text.Json;
using Dating.API.Helpers;
using Dating.Shared;

namespace Dating.API.Extensions;

public static class HttpExtensions
{
    public static void ApplyPagination<T>(this HttpResponse response, PageList<T> data)
    {
        var paginationHeader = new PaginationHeader(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
        var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(paginationHeader, options);
        response.Headers.Append("Pagination", json);
        response.Headers.Append("Access-Control-Expose-Headers", "Pagination");
    }
}