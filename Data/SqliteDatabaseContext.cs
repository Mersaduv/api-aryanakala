using Microsoft.EntityFrameworkCore;

namespace ApiAryanakala.Data;

public class SqliteDatabaseContext(DbContextOptions dbContextOptions) : ApplicationDbContext(dbContextOptions);
