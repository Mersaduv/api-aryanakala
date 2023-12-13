using ApiAryanakala;
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
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//fill configs from appsetting.json
builder.Services.AddOptions();
builder.Services.Configure<Configs>(builder.Configuration.GetSection("Configs"));

builder.Services.AddRepositories();
builder.Services.AddUnitOfWork();
builder.Services.AddInfraUtility();
builder.Services.AddScoped<IProductServices, ProductServices>();

builder.Services.AddApplicationServices();
// builder.Services.AddStackExchangeRedisCache(options =>
//    {
//        options.Configuration = configuration["RedisCache:Url"];
//        options.InstanceName = configuration["RedisCache:Prefix"];
//    });
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379,password=redis";
    options.InstanceName = "aryanaKala";
});

string connectionString = builder.Configuration.GetConnectionString("SqlConnection");

//register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddJWT();
builder.Services.AddSwagger();
builder.Services.AddCors();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddSingleton<ByteFileUtility>();
builder.Services.AddHttpContextAccessor();

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

app.ConfigureAuthEndpoints();
app.ConfigureProductEndpoints();
app.ConfigureCategoryEndpoints();
app.ConfigureBrandEndpoints();
app.UseHttpsRedirection();

app.Run();
