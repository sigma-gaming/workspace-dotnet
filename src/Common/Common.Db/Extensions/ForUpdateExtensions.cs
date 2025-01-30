using Microsoft.EntityFrameworkCore;

namespace Common.Db.Extensions;

public static class ForUpdateExtensions
{
    public static IQueryable<T> ForUpdate<T>(this IQueryable<T> query) where T : class
    {
        return query.TagWith("ForUpdate");
    }
}