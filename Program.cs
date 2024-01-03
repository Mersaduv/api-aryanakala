using ApiAryanakala;
using ApiAryanakala.Const;
using ApiAryanakala.Data;
using ApiAryanakala.Endpoints;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;
using ApiAryanakala.Services.Product;
using ApiAryanakala.Utility;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ApplicationName = typeof(Program).Assembly.FullName,
    ContentRootPath = Path.GetFullPath(Directory.GetCurrentDirectory()),
    WebRootPath = Path.GetFullPath(Directory.GetCurrentDirectory()),
    Args = args
});
var config = builder.Configuration;
var appSettings = config.Get<AppSettings>() ?? new AppSettings();

var services = builder.Services;
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
//fill configs from appsetting.json
services.AddOptions();
services.Configure<Configs>(builder.Configuration.GetSection("Configs"));

services.AddRepositories();
services.AddUnitOfWork();
services.AddInfraUtility();
services.AddScoped<IProductServices, ProductServices>();

services.AddApplicationServices();
// services.AddStackExchangeRedisCache(options =>
//    {
//        options.Configuration = configuration["RedisCache:Url"];
//        options.InstanceName = configuration["RedisCache:Prefix"];
//    });
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379,password=redis";
    options.InstanceName = "aryanaKala";
});

string connectionString = builder.Configuration.GetConnectionString("SqlConnection");

//register DbContext
services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});
_ = appSettings.DbProvider switch
{
    null => throw new Exception("No provider found"),

    DbProvider.Sqlite => services.AddDbContext<ApplicationDbContext, SqliteDatabaseContext>(options =>
        options.UseSqlite(config.GetConnectionString(nameof(DbProvider.Sqlite)) ?? string.Empty)),

    DbProvider.SqlServer => services.AddDbContext<ApplicationDbContext, SqlServerDatabaseContext>(options =>
        options.UseSqlServer(config.GetConnectionString(nameof(DbProvider.SqlServer)) ?? string.Empty)),

    DbProvider.PostgreSql => services.AddDbContext<ApplicationDbContext, PostgreSqlDatabaseContext>(options =>
        options.UseNpgsql(config.GetConnectionString(nameof(DbProvider.PostgreSql)) ?? string.Empty)),

    DbProvider.InMemory => services.AddDbContext<ApplicationDbContext, InMemoryDatabaseContext>(options =>
        options.UseInMemoryDatabase(nameof(InMemoryDatabaseContext))),

    DbProvider.AzureCosmos => services.AddDbContext<ApplicationDbContext, AzureCosmosDatabaseContext>(options =>
        options.UseCosmos(config.GetConnectionString(nameof(DbProvider.AzureCosmos)) ?? string.Empty,
            databaseName: nameof(DbProvider.AzureCosmos))),

    _ => throw new Exception($"Unsupported provider: {appSettings.DbProvider}")
};

services.AddValidatorsFromAssembly(typeof(Program).Assembly);
services.AddJWT();
services.AddSwagger();
services.AddCors();

services.AddAuthentication();
services.AddAuthorization();
services.AddSingleton<ByteFileUtility>();
services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api Aryanakala"));
// }
// app.UseCors(b => b.AllowAnyOrigin());
// app.UseCors();
app.UseCors(b => b.AllowAnyOrigin().AllowAnyMethod());
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

var apiGroup = app.MapGroup(Constants.Api);
apiGroup
    .MapAuthApi()
    .MapProductApi()
    .MapAddressApi()
    .MapBrandApi()
    .MapCartApi()
    .MapCategoryApi()
    .MapOrderApi()
    .MapPaymentApi()
    .MapRatingApi();

app.UseHttpsRedirection();

app.Run();
