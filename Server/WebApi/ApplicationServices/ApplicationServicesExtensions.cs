using System.Reflection;
using System.Text;
using DataAccess;
using Domain.Entities;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using WebApi.Configuration;
using WebApi.Services.GameService;
using WebApi.Services.Jwt;
using WebApi.Services.Mongo;
using WebApi.Services.RabbitMq;

namespace WebApi.ApplicationServices;

public static class ApplicationServicesExtensions
{
    private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
    
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services.AddScoped<IGameService, GameService>();
    }
    
    public static IServiceCollection AddMediatr(this IServiceCollection services)
    {
        return services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly));
    }
    
    public static IServiceCollection AddApplicationDb(this IServiceCollection services,
        string? connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.LogTo(Console.WriteLine);
            options.UseNpgsql(connectionString);
        });

        return services;
    }

    public static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddIdentityCore<User>(options =>
            {
                options.User.RequireUniqueEmail = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 1;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddApiEndpoints();

        return services;
    }
    
    public static IServiceCollection AddJWTAuthorization(this IServiceCollection services, IConfigurationManager config)
    {
        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = config["JwtSettings:Issuer"],
                    ValidAudience = config["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:Key"]!)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true, //Todo надо ли это и нужно ли делать refresh для этого?
                    ValidateIssuerSigningKey = true
                };
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        
                        if (!string.IsNullOrEmpty(accessToken) &&
                           path.StartsWithSegments("/gamehub"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorizationBuilder();
        
        services.AddScoped<IJwtGenerator, JwtGenerator>();
        
        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        });
        
        return services;
    }

    public static IServiceCollection AddMongo(this IServiceCollection services, MongoDbConfig mongoConfiguration)
    {
        var mongoClient = new MongoClient(mongoConfiguration.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoConfiguration.DatabaseName);

        var ratingCollection = mongoDatabase.GetCollection<Rating>(mongoConfiguration.CollectionName);
        return services.AddTransient<IRatingRepository, RatingRepository>(_ => new RatingRepository(ratingCollection));
    }
    
    public static IServiceCollection AddMassTransitAndRabbitMq(this IServiceCollection services, 
        RabbitMqConfig rabbitMqConfig)
    {
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.AddConsumer<GameResultsConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(new Uri(rabbitMqConfig.Host), h =>
                {
                    h.Username(rabbitMqConfig.Username);
                    h.Password(rabbitMqConfig.Password);
                });
        
                cfg.ConfigureEndpoints(ctx);
            });
        });
        
        return services;
    }
}