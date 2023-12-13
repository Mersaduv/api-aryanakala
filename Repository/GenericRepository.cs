using ApiAryanakala.Data;
using ApiAryanakala.Interfaces.IRepository;
using ApiAryanakala.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiAryanakala.Repository;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseClass<int>
{
    private readonly ApplicationDbContext _context;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<int> Add(T entity)
    {
        _context.Set<T>().Add(entity);
        return await _context.SaveChangesAsync();
    }

    public async Task<int> Update(T entity)
    {
        _context.Set<T>().Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        return await _context.SaveChangesAsync();
    }

    public async Task<int> Delete(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
        {
            return 0; // Entity not found
        }

        _context.Set<T>().Remove(entity);
        return await _context.SaveChangesAsync();
    }
}