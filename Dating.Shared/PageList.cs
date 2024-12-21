using Microsoft.EntityFrameworkCore;

namespace Dating.Shared;

// response that returns when i user pageination
public class PageList<T> : List<T>
{
    private PageList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        CurrentPage = pageNumber;
        TotalCount = count;
        PageSize = pageSize;
        AddRange(items);
    }

    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int TotalCount { get; set; }
    public int PageSize { get; set; }

    public static async Task<PageList<T>> CreateAsync(IQueryable<T> source, int pagenumber, int pagesize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToListAsync();
        return new PageList<T>(items, count, pagenumber, pagesize);
    }
}