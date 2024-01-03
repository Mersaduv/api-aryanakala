using Microsoft.EntityFrameworkCore;

namespace ApiAryanakala.Data;

public class AzureCosmosDatabaseContext(DbContextOptions dbContextOptions) : ApplicationDbContext(dbContextOptions);
