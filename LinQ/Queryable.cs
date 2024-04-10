using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace ApiAryanakala.LinQ;

public static class Queryable
{
    /// <summary>
    /// Filter by condition
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="condition"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
    {
        if (condition)
        {
            return source.Where(predicate);
        }
        else
        {
            return source;
        }
    }

    /// <summary>
    /// Get query paged
    /// </summary>
    /// <typeparam name="TSource">Source type</typeparam>
    /// <param name="source">Source query</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Query items of page</returns>
    public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> source, int page, int pageSize)
    {
        return source.Skip((page - 1) * pageSize).Take(pageSize);
    }

    /// <summary>
    /// Queryable orderby extension
    /// </summary>
    /// <typeparam name="T">Resource</typeparam>
    /// <param name="query">Query resource</param>
    /// <param name="propertyName">Order by property name</param>
    /// <param name="isAscend">Order by ascend</param>
    /// <returns></returns>
    public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyName, bool isAscend)
    {
        if (!typeof(T).HasProperty(propertyName))
        {
            return query;
        }
        if (isAscend)
        {
            query = query.OrderBy(p => EF.Property<object>(p!, propertyName));
        }
        else
        {
            query = query.OrderByDescending(p => EF.Property<object>(p!, propertyName));
        }
        return query;
    }

    private static bool HasProperty(this Type obj, string propertyName)
    {
        return !string.IsNullOrEmpty(propertyName) && obj.GetProperty(propertyName) != null;
    }
}
