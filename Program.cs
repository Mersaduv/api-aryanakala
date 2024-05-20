using ApiAryanakala;
using ApiAryanakala.Const;
using ApiAryanakala.Data;
using ApiAryanakala.Endpoints;
using ApiAryanakala.Models;
using FluentValidation;
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
builder.Services.AddMemoryCache();
builder.Services.Configure<JsonOptions>(options =>
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
// app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api Aryanakala"));
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api Aryanakala");
});
// }
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
    .MapReviewApi()
    .MapSliderApi()
    .MapBannerApi()
    .MapDetailsApi();

app.UseHttpsRedirection();

app.Run();
