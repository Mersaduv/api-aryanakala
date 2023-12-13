using ApiAryanakala.Data;
using ApiAryanakala.Entities;
using ApiAryanakala.Entities.Exceptions;
using ApiAryanakala.Interfaces.IServices;
using Microsoft.EntityFrameworkCore;

namespace ApiAryanakala.Services.Product;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Category> Add(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<bool> Delete(int id)
    {
        var success = false;
        Category category = await GetBy(id);
        if (category == null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            success = true;
        }
        return success;
    }

    public async Task<Category> GetBy(int id)
    {
        return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Category>> GetAll()
    {
        var categories = await _context.Categories.ToListAsync();
        return categories;
    }

    public async Task<Category> Update(Category category)
    {
        var dbCategory = await GetBy(category.Id);
        if (dbCategory == null)
        {
            throw new CoreException("Category not found.");
        }

        dbCategory.Name = category.Name;
        dbCategory.Url = category.Url;

        await _context.SaveChangesAsync();

        return dbCategory;

    }

}