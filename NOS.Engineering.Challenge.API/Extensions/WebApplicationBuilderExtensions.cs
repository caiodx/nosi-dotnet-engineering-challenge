using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.OpenApi.Models;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;
using NOS.Engineering.Challenge.Services;
using Serilog;
using static NOS.Engineering.Challenge.Utils.Enums;

namespace NOS.Engineering.Challenge.API.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder webApplicationBuilder)
    {
        var serviceCollection = webApplicationBuilder.Services;
        var configuration = webApplicationBuilder.Configuration;

        serviceCollection.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.PropertyNamingPolicy = null;
        });
        serviceCollection.AddControllers();
        serviceCollection
            .AddEndpointsApiExplorer();

        serviceCollection.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Nos Challenge Api", Version = "v1" });
        });

        serviceCollection
            .RegisterSlowDatabase()
            .RegisterContentsManager()
            .RegisterDatabase()
            .RegisterCache(configuration);

        webApplicationBuilder.ConfigureLogging();

        return webApplicationBuilder;
    }

    private static IServiceCollection RegisterSlowDatabase(this IServiceCollection services)
    {
        services.AddSingleton<IDatabase<Content, ContentDto>,SlowDatabase<Content, ContentDto>>();
        services.AddSingleton<IMapper<Content, ContentDto>, ContentMapper>();
        services.AddSingleton<IMockData<Content>, MockData>();

        return services;
    }
    
    private static IServiceCollection RegisterContentsManager(this IServiceCollection services)
    {
        services.AddSingleton<IContentsManager, ContentsManager>();

        return services;
    }
    
    
    public static WebApplicationBuilder ConfigureWebHost(this WebApplicationBuilder webApplicationBuilder)
    {
        //webApplicationBuilder
        //    .WebHost
        //    .ConfigureLogging(logging => { logging.ClearProviders(); });

        return webApplicationBuilder;
    }

    private static IServiceCollection RegisterDatabase(this IServiceCollection services)
    {
        services.AddSingleton<IMongoDatabase<Content, ContentDto>, MongoDatabase<Content, ContentDto>>();

        return services;
    }

    private static IServiceCollection RegisterCache(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            string connections = configuration.GetConnectionString("Redis")!;
            options.Configuration = connections;
        });
        services.AddSingleton<IRedisCacheService, RedisCacheService>();

        return services;
    }

    public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder webApplicationBuilder)
    {
        LoggerConfiguration loggerConfiguration = new();
        loggerConfiguration.MinimumLevel.Debug();

        switch (GetAppEnviroment())
        {
            case AppEnviroment.Development:
                loggerConfiguration.WriteTo.Console();
                break;
            case AppEnviroment.Production:
                loggerConfiguration.WriteTo.File("logs/apilogs-.txt", rollingInterval: RollingInterval.Day);
                break;
        }

        Log.Logger = loggerConfiguration.CreateLogger();
        return webApplicationBuilder;
    }

}