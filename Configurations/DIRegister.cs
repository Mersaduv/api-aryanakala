using System.Text;
using ApiAryanakala.Framework;
using ApiAryanakala.Interfaces;
using ApiAryanakala.Models;
using ApiAryanakala.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ApiAryanakala;

public static class DIRegister
{
    // public static void AddRepositories(this IServiceCollection services)
    // {
    //     services.AddScoped<IProductRepository, ProductRepository>();
    //     //services.AddScoped<IProductRepository, ProductRepository>();
    //     //services.AddScoped<IProductRepository, ProductRepository>();
    //     //services.AddScoped<IProductRepository, ProductRepository>();
    //     //services.AddScoped<IProductRepository, ProductRepository>();
    //     //services.AddScoped<IProductRepository, ProductRepository>();
    // }

    public static void AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    public static void AddInfraUtility(this IServiceCollection services)
    {
        services.AddSingleton<EncryptionUtility>();
    }

    public static IServiceCollection AddJWT(this IServiceCollection services)
    {
        var sp = services.BuildServiceProvider();
        Configs configs = sp.GetService<IOptions<Configs>>().Value;
        var key = Encoding.UTF8.GetBytes(configs.TokenKey);

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.FromMinutes(configs.TokenTimeout),
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Api Aryanakala",
                Description = "Api Aryanakala - Version01",
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "Bearer",
                Name = "Authorization",
                In = ParameterLocation.Header,
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });

            c.EnableAnnotations();
        });
        return services;
    }
}