using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MusicDownloader.Business.Logic;
using MusicDownloader.Shared.Constants;
using MusicDownloader.Shared.Options;
using Serilog;
using YoutubeReExplode;

namespace MusicDownloader.Startup;

public static class DependencyRegistration
{
    public static void ConfigureAuthentication(this IServiceCollection serviceCollection)
    {
        // Get Jwt options
        var jwtOptions = new JwtOptions();
        jwtOptions.SetFromEnvironmentVariables();

        // JWT Authentication
        serviceCollection
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = new TimeSpan(0),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
                };
            });
    }
    public static void ConfigureSwagger(this IServiceCollection serviceCollection)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        serviceCollection.AddSwaggerGen(options =>
        {
            // Define JWT Authentication
            options.AddSecurityDefinition(ApplicationConstants.Jwt.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme.",
                Name = ApplicationConstants.Jwt.HeaderName,
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = ApplicationConstants.Jwt.BearerPrefix
            });
            // Require JWT Authentication in swagger
            options.AddSecurityRequirement(new OpenApiSecurityRequirement // Is key-value pair dictionary
            {
                // Object initializer
                {
                    // key-value pair
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = ApplicationConstants.Jwt.AuthenticationScheme
                        }
                    },
                    new List<string>()
                }
            });
        });
    }

    public static void AddBusiness(this IServiceCollection serviceCollection)
    {
        // Add business services here
        serviceCollection.AddScoped<JwtTokenGenerator>();
    }

    public static void AddLibraries(this IServiceCollection serviceCollection)
    {
        // Add serilog
        serviceCollection.AddSingleton(Log.Logger);

        // Add YoutubeReExplode
        serviceCollection.AddScoped<YoutubeClient>();

        // Automapper
        serviceCollection.AddAutoMapper(typeof(Program));

        // MediatR
        serviceCollection.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
    }
}