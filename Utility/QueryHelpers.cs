using ApiAryanakala.Entities.Exceptions;
using ApiAryanakala.Entities.Product;
using ApiAryanakala.Enums;
using ApiAryanakala.Extensions;
using ApiAryanakala.Models;
using System.Linq.Expressions;
namespace ApiAryanakala.Utility;

public static class QueryHelpers
{
    public static IQueryable<T> BuildQuery<T>(IQueryable<T> query, RequestQueryParameters parameters) where T : Product
    {
        if (parameters.Sort.IsNullEmpty().Not() && parameters.SortBy.IsNullEmpty().Not())
        {
            query = BuildOrder(query, parameters.Sort!, parameters.SortBy!);
        }
        query = ApplyPriceRange(query, parameters.MinPrice, parameters.MaxPrice);

        return query;
    }

    public static bool IsOrdered<T>(this IQueryable<T> queryable)
    {
        if (queryable == null)
        {
            throw new ArgumentNullException(nameof(queryable));
        }

        return queryable.Expression.Type == typeof(IOrderedQueryable<T>);
    }
    public static IQueryable<T> BuildPagination<T>(IQueryable<T> query, RequestQueryParameters parameters)
    {
        if (parameters.Page != default && parameters.PageSize != default)
        {
            query = query.Skip((parameters.Page - 1) * parameters.PageSize).Take(parameters.PageSize);
        }
        return query;
    }
    private static IQueryable<T> BuildOrder<T>(IQueryable<T> query, string sort, string sortBy)
    {
        if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(sort))
        {
            var entityType = typeof(T);
            var propertyName = sortBy;

            // Get the property based on the provided sortBy
            var property = entityType.GetProperty(propertyName);

            if (property != null)
            {
                var parameter = Expression.Parameter(entityType, "x");

                var propertyAccess = Expression.Property(parameter, property);

                var orderByExpression = Expression.Lambda(propertyAccess, parameter);

                var methodName = sort.ToLower() == OrderTypes.desc.ToString() ? "OrderByDescending" : "OrderBy";

                // Create an expression representing the OrderBy or OrderByDescending method call
                var orderByMethod = typeof(Queryable).GetMethods()
                    .Single(method => method.Name == methodName && method.IsGenericMethodDefinition && method.GetParameters().Length == 2)
                    .MakeGenericMethod(entityType, property.PropertyType);

                // Call OrderBy or OrderByDescending on the query
                query = (IQueryable<T>)orderByMethod.Invoke(null, new object[] { query, orderByExpression });
            }
            else
            {
                throw new CoreException($"Property '{sortBy}' not found in entity type '{entityType.Name}'.");
            }
        }

        return query;
    }

    private static IQueryable<T> ApplyPriceRange<T>(IQueryable<T> query, double? minPrice, double? maxPrice) where T : Product
    {
        if (minPrice.HasValue)
        {
            query = query.Where(item => item.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(item => item.Price <= maxPrice.Value);
        }
        return query;
    }



}