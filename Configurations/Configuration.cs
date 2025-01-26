using Bogus;
using y_nuget.RabbitMq;
using YprojectUserService.Authorization;
using YprojectUserService.Database;
using YprojectUserService.Razor;

namespace YprojectUserService.Configurations;

public static class Configuration
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services, WebApplicationBuilder builder)
    {   
        var configuration = builder.Configuration;
        
        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMQ"));
        services.AddDataBaseConfig(configuration);
        services.AddHttpContextAccessor();
        services.AddSwaggerGen();
        services.AddAuthConfig();
        services.AddRabbitMqConfig();
        builder.Services.AddSingleton<Faker>();
        builder.Services.AddTransient<RazorRenderer>();

        return services;
    }   
}       