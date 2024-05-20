using ApiAryanakala.Data;
using ApiAryanakala.Entities.Product;
using ApiAryanakala.Interfaces.IRepository;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Mapper;
using ApiAryanakala.Utility;
using Microsoft.EntityFrameworkCore;
namespace ApiAryanakala.Repository;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ByteFileUtility _byteFileUtility;
    private readonly ICategoryService _categoryService;

    public ProductRepository(ApplicationDbContext context, ByteFileUtility byteFileUtility, ICategoryService categoryService)
    {
        _context = context;
        _byteFileUtility = byteFileUtility;
        _categoryService = categoryService;
    }
    public async Task CreateAsync(Product product)
    {
        await _context.AddAsync(product);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        var productAll = _context.Products.Include(x => x.Info)
        .Include(x => x.Specifications)
        .Include(x => x.Images)
        .Include(x => x.Colors)
        .Include(x => x.Review)
        .Include(c=>c.Category)
        .ThenInclude(x=>x.ParentCategory)
        .AsNoTracking()
        .ToListAsync();

        return await productAll;
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _context.Products.CountAsync();
    }

    public async Task<Product?> GetAsyncBy(Guid id)
    {
        return await _context.Products.Include(x => x.Info)
        .Include(x=> x.Category)
        .ThenInclude(x=>x.ParentCategory)
        .Include(x => x.Specifications)
        .Include(x => x.Images)
        .Include(x => x.Colors)
        .Include(x => x.Review)
        .AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Product?> GetAsyncBy(string productName)
    {
        return await _context.Products.Include(x => x.Info)
        .Include(x => x.Specifications)
        .Include(x => x.Images)
        .Include(x => x.Colors)
        .Include(x => x.Review)
        .FirstOrDefaultAsync(u => u.Title.ToLower() == productName.ToLower());
    }

    public async Task<IEnumerable<Product>> GetProductsAsyncBy(string categoryUrl)
    {
        var category = _context.Categories
                              .Where(p => p.Url.ToLower().Equals(categoryUrl.ToLower()))
                              .Include(p => p.ChildCategories)
                              .FirstOrDefault();
        var productsFromDB = await GetAllAsync();

        if (category != null)
        {
            List<int> categoriesToSearchForProducts = new List<int>();
            categoriesToSearchForProducts.Add(category.Id);

            var childCategories = _categoryService.GetAllChildCategories(category.Id);
            categoriesToSearchForProducts.AddRange(childCategories);

            productsFromDB = productsFromDB.Where(p => categoriesToSearchForProducts.Contains(p.CategoryId));
        }
        // return products;
        return productsFromDB;
    }


    public void Remove(Product product)
    {
        _context.Products.Remove(product);
    }

    public void Update(Product product)
    {
        _context.Products.Update(product);
    }

    public async Task<ProductSearchResult> SearchProductsAsync(string searchText, int page)
    {
        var pageResults = 2f;
        var pageCount = Math.Ceiling((await FindProductsBySearchText(searchText)).Count / pageResults);
        IEnumerable<Product> products = await _context.Products
                            .Where(p => p.Title.ToLower().Contains(searchText.ToLower().Trim()) ||
                                p.Description.ToLower().Contains(searchText.ToLower().Trim()))
                            .Include(x => x.Info).Include(x => x.Specifications).Include(x => x.Images).Include(x => x.Colors)
                            .Skip((page - 1) * (int)pageResults)
                            .Take((int)pageResults)
                            .ToListAsync();
        var result = products.ToProductsResponse(_byteFileUtility);
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
        return await _context.Products
                            .Where(p => p.Title.ToLower().Contains(searchText.ToLower().Trim()) ||
                                p.Description.ToLower().Contains(searchText.ToLower().Trim()))
                            .Include(x => x.Info).Include(x => x.Specifications).Include(x => x.Images).Include(x => x.Colors)
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