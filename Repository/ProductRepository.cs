using ApiAryanakala.Data;
using ApiAryanakala.Entities;
using ApiAryanakala.Interfaces.IRepository;
using ApiAryanakala.Models.DTO.ProductDto;
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
        var productWithInfo = db.Products.Include(x => x.Info)
        .AsNoTracking()
        .ToListAsync();
        // .Select(x => new ProductDTO
        // {
        //     Id = x.Id,
        //     Title = x.Title,
        //     Info = x.Info.Select(infoDto => new InfoDto
        //     {
        //         Title = infoDto.Title,
        //         Value = infoDto.Value,
        //     }).ToList(),
        //     Category = x.Category,
        //     Description = x.Description,
        //     Discount = x.Discount,
        //     Images = x.Images,
        //     InStock = x.InStock,
        //     Price = x.Price,
        //     Rating = x.Rating,
        //     Size = x.Size,
        //     Slug = x.Slug,
        //     Sold = x.Sold
        // })


        return await productWithInfo;
    }


    public async Task<Product> GetAsync(Guid id)
    {
        return await db.Products.Include(x => x.Info).FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Product> GetAsync(string productName)
    {
        return await db.Products.FirstOrDefaultAsync(u => u.Title.ToLower() == productName.ToLower());
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