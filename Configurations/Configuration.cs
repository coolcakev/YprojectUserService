using Bogus;
using y_nuget;
using y_nuget.RabbitMq;
using YprojectUserService.Razor;
using YprojectUserService.Database;
using YprojectUserService.Authorization;

namespace YprojectUserService.Configurations;

public static class Configuration
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services, WebApplicationBuilder builder)
    {   
        var configuration = builder.Configuration;
        
        services.AddDataBaseConfig(configuration);
        services.AddHttpContextAccessor();
        services.AddSwaggerGen();
        services.AddAuthConfig();
        services.AddRabbitMqConfig(builder);
        services.AddYNugetConfiguration(builder);
        services.AddSingleton<Faker>();
        services.AddTransient<RazorRenderer>();

        return services;
    }   
}       