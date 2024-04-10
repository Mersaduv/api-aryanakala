using System.Text;
using ApiAryanakala.Framework;
using ApiAryanakala.Interfaces;
using ApiAryanakala.Interfaces.IRepository;
using ApiAryanakala.Interfaces.IServices;
using ApiAryanakala.Models;
using ApiAryanakala.Repository;
using ApiAryanakala.Services;
using ApiAryanakala.Services.Auth;
using ApiAryanakala.Services.Product;
using ApiAryanakala.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ApiAryanakala
{
    public static class DIRegister
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>()
                    .AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        }
        public static void AddApplicationServices(this IServiceCollection services, AppSettings appSettings)
        {
            services.AddScoped(_ => appSettings)
                    .AddScoped<IProductServices, ProductServices>()
                    .AddScoped<IPermissionService, PermissionService>()
                    .AddScoped<IAuthServices, AuthService>()
                    .AddScoped<ICategoryService, CategoryService>()
                    .AddScoped<IReviewServices, ReviewServices>()
                    .AddScoped<IAddressService, AddressService>()
                    .AddScoped<ICartService, CartService>()
                    .AddScoped<IOrderServices, OrderService>()
                    .AddScoped<ISliderServices, SliderServices>()
                    .AddScoped<IDetailsServices, DetailsServices>()
                    .AddScoped<IBannerServices, BannerServices>();
            // services.AddScoped<IPaymentService, PaymentService>();
        }

        public static void AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
        public static void AddInfraUtility(this IServiceCollection services)
        {
            services.AddScoped<ByteFileUtility>();
        }

        public static IServiceCollection AddJWT(this IServiceCollection services, AppSettings appSettings)
        {
            var sp = services.BuildServiceProvider();
            var key = Encoding.UTF8.GetBytes(appSettings.AuthSettings.TokenKey);

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
                    ClockSkew = TimeSpan.FromMinutes(appSettings.AuthSettings.TokenTimeout),
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

}