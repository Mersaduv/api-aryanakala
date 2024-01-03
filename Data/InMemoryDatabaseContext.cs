using Microsoft.EntityFrameworkCore;

namespace ApiAryanakala.Data;

public class InMemoryDatabaseContext(DbContextOptions dbContextOptions) : ApplicationDbContext(dbContextOptions);
