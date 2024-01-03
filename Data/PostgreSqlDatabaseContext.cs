using Microsoft.EntityFrameworkCore;

namespace ApiAryanakala.Data;

public class PostgreSqlDatabaseContext(DbContextOptions dbContextOptions) : ApplicationDbContext(dbContextOptions);
