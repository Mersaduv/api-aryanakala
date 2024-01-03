using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiAryanakala.LinQ;

public static class Enumerable
{
    /// <summary>
    /// Filter by condition
    /// </summary>
    /// <typeparam name="TSource">Source type</typeparam>
    /// <param name="source">Source</param>
    /// <param name="condition">Condition</param>
    /// <param name="predicate">Predicate</param>
    /// <returns></returns>
    public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
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
    /// Get items paged
    /// </summary>
    /// <typeparam name="TSource">Source type</typeparam>
    /// <param name="source">Data source</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>List items of page</returns>
    public static IEnumerable<TSource> Page<TSource>(this IEnumerable<TSource> source, int page, int pageSize)
    {
        return source.Skip((page - 1) * pageSize).Take(pageSize);
    }

    /// <summary>
    /// Copy from
    /// https://github.com/morelinq/MoreLINQ
    /// </summary>
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer = null)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (keySelector == null)
            throw new ArgumentNullException(nameof(keySelector));

        return _();
        IEnumerable<TSource> _()
        {
            var knownKeys = new HashSet<TKey>(comparer);
            foreach (var element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                    yield return element;
            }
        }
    }
}
