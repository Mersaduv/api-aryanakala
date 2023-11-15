using ApiAryanakala.Data;
using Mapster;
using ApiAryanakala.Entities.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace ApiAryanakala.Utility;

public class PaginatedList<T, K>
{
    public int PageIndex { get; protected set; }
    public int TotalPages { get; protected set; }
    public List<K> Data { get; protected set; }
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;
    public bool Paginated { get; protected set; }
    public PaginatedList(List<K> items, int count, int pageIndex, int pageSize, bool paginated = true)
    {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        Paginated = paginated;
        Data = new List<K>(items);
    }
    public static async Task<PaginatedList<T, K>> CreateAsync(IQueryable<T> source, int? pageIndex, int pageSize)
    {
        var idx = pageIndex.HasValue ? pageIndex.Value : 1;
        var count = await source.CountAsync();
        var isOrdered = source.IsOrdered();

        if (!isOrdered)
        {
            try
            {
                var modelType = typeof(T);
                var context = (ApplicationDbContext)ApplicationDbContext.GetDbContext(source);
                var orderByProperty = context?.GetPrimaryKey(modelType);

                var property = modelType.GetProperty(orderByProperty);
                var parameter = Parameter(modelType, "p");
                var propertyAccess = MakeMemberAccess(parameter, property);
                var convertedPropertyAccess = Expression.Convert(propertyAccess, typeof(object));
                var expr = Lambda<Func<T, object>>(convertedPropertyAccess, parameter);

                source = source.OrderBy(expr);
            }
            catch (Exception ex)
            {
               throw new CoreException("Failed to add order by to query. Message: " + ex.Message);
            }
        }
        var items = await source.Skip((idx - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<K>()
            .ToListAsync();
        return new PaginatedList<T, K>(items, count, idx, pageSize);
    }
}