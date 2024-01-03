using Microsoft.EntityFrameworkCore;

namespace ApiAryanakala.Data;

public class SqlServerDatabaseContext(DbContextOptions dbContextOptions) : ApplicationDbContext(dbContextOptions);
