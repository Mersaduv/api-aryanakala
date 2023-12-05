using ApiAryanakala.Data;
using ApiAryanakala.Entities;
using ApiAryanakala.Interfaces.IRepository;
using ApiAryanakala.Models.DTO.ProductDtos;
using Microsoft.EntityFrameworkCore;

namespace ApiAryanakala.Repository;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext db;


    public ProductRepository(ApplicationDbContext db)
    {
        this.db = db;

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

    public async Task RemoveAsync(Product product)
    {
        db.Products.Remove(product);
    }

    public async Task UpdateAsync(Product product)
    {
        db.Products.Update(product);
    }

}