using Bogus;
using MassTransit;
using YprojectUserService.Authorization;
using YprojectUserService.Database;
using YprojectUserService.Razor;

namespace YprojectUserService.Configurations;

public static class Configuration
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services, WebApplicationBuilder builder)
    {   
        var configuration = builder.Configuration;
        
        services.AddDataBaseConfig(configuration);
        services.AddCorsConfig();
        services.AddHttpContextAccessor();
        services.AddSwaggerGen();
        services.AddAuthConfig();
        builder.Services.AddSingleton<Faker>();
        builder.Services.AddTransient<RazorRenderer>();
        builder.Services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
            });
        });

        return services;
    }   
}       