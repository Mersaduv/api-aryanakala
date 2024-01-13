using ApiAryanakala;
using ApiAryanakala.Const;
using ApiAryanakala.Data;
using ApiAryanakala.Endpoints;
using ApiAryanakala.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ApplicationName = typeof(Program).Assembly.FullName,
    ContentRootPath = Path.GetFullPath(Directory.GetCurrentDirectory()),
    WebRootPath = Path.GetFullPath(Directory.GetCurrentDirectory()),
    Args = args
});
var config = builder.Configuration;
var appSettings = config.Get<AppSettings>() ?? new AppSettings();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//fill configs from appsetting.json
builder.Services.AddOptions();

builder.Services.AddRepositories();
builder.Services.AddUnitOfWork();

builder.Services.AddApplicationServices(appSettings);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = appSettings.RedisCache.Url;
    options.InstanceName = appSettings.RedisCache.Prefix;
});


string connectionString = builder.Configuration.GetConnectionString("SqlConnection");

//register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddJWT(appSettings);
builder.Services.AddSwagger();
builder.Services.AddCors();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddInfraUtility();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api Aryanakala"));
// }
app.UseCors(b => b.AllowAnyOrigin().AllowAnyMethod());
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

var apiGroup = app.MapGroup(Constants.Api);
apiGroup
    .MapAuthApi()
    .ConfigureProductEndpoints()
    .MapAddressApi()
    .MapBrandApi()
    .MapCartApi()
    .MapCategoryApi()
    .MapOrderApi()
    .MapPaymentApi()
    .MapRatingApi();
app.UseHttpsRedirection();

app.Run();
