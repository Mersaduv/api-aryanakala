using ApiAryanakala;
using ApiAryanakala.Data;
using ApiAryanakala.Endpoints;
using ApiAryanakala.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//fill configs from appsetting.json
builder.Services.AddOptions();
builder.Services.Configure<Configs>(builder.Configuration.GetSection("Configs"));

// builder.Services.AddRepositories();
builder.Services.AddUnitOfWork();
builder.Services.AddInfraUtility();

string connectionString = builder.Configuration.GetConnectionString("SqlConnection");

//register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddJWT();
builder.Services.AddSwagger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseAuthentication();
// app.UseAuthorization();

app.ConfigureAuthEndpoints();
app.UseHttpsRedirection();

app.Run();
