using ApiAryanakala.Data;
using ApiAryanakala.Entities;
using ApiAryanakala.Interfaces.IRepository;
using ApiAryanakala.Mapper;
using ApiAryanakala.Utility;
using Microsoft.EntityFrameworkCore;
namespace ApiAryanakala.Repository;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext db;
    private readonly ByteFileUtility byteFileUtility;

    public ProductRepository(ApplicationDbContext db, ByteFileUtility byteFileUtility)
    {
        this.db = db;
        this.byteFileUtility = byteFileUtility;
    }
    public async Task CreateAsync(Product product)
    {
        await db.AddAsync(product);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        var productAll = db.Products.Include(x => x.Info).Include(x => x.Images)
        .AsNoTracking()
        .ToListAsync();

        return await productAll;
    }


    public async Task<Product> GetAsyncBy(Guid id)
    {
        return await db.Products.Include(x => x.Info).Include(x => x.Images).FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Product> GetAsyncBy(string productName)
    {
        return await db.Products.Include(x => x.Info).Include(x => x.Images).FirstOrDefaultAsync(u => u.Title.ToLower() == productName.ToLower());
    }

    public async Task<IEnumerable<Product>> GetProductsAsyncBy(string categoryUrl)
    {
        return await db.Products
                              .Where(p => p.Category.Url.ToLower().Equals(categoryUrl.ToLower()))
                              .Include(p => p.Info)
                              .Include(p => p.Images)
                              .ToListAsync();
    }


    public async Task RemoveAsync(Product product)
    {
        db.Products.Remove(product);
    }

    public async Task UpdateAsync(Product product)
    {
        db.Products.Update(product);
    }

    public async Task<ProductSearchResult> SearchProductsAsync(string searchText, int page)
    {
        var pageResults = 2f;
        var pageCount = Math.Ceiling((await FindProductsBySearchText(searchText)).Count / pageResults);
        IEnumerable<Product> products = await db.Products
                            .Where(p => p.Title.ToLower().Contains(searchText.ToLower()) ||
                                p.Description.ToLower().Contains(searchText.ToLower()))
                            .Include(x => x.Info).Include(x => x.Images)
                            .Skip((page - 1) * (int)pageResults)
                            .Take((int)pageResults)
                            .ToListAsync();
        var result = products.ToProductsResponse(byteFileUtility);
        var response = new ProductSearchResult
        {

            Products = result,
            CurrentPage = page,
            Pages = (int)pageCount
        };

        return response;
    }

    public async Task<List<Product>> FindProductsBySearchText(string searchText)
    {
        return await db.Products
                            .Where(p => p.Title.ToLower().Contains(searchText.ToLower()) ||
                                p.Description.ToLower().Contains(searchText.ToLower()))
                            .Include(x => x.Info).Include(x => x.Images)
                            .ToListAsync();
    }

    public async Task<List<string>> GetProductSearchSuggestionsAsync(string searchText)
    {
        var products = await FindProductsBySearchText(searchText);

        List<string> result = new List<string>();

        foreach (var product in products)
        {
            if (product.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            {
                result.Add(product.Title);
            }

            if (product.Description != null)
            {
                var punctuation = product.Description.Where(char.IsPunctuation)
                    .Distinct().ToArray();
                var words = product.Description.Split()
                    .Select(s => s.Trim(punctuation));

                foreach (var word in words)
                {
                    if (word.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                        && !result.Contains(word))
                    {
                        result.Add(word);
                    }
                }
            }
        }
        return result;
    }

}