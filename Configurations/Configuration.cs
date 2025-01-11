using Bogus;
using MassTransit;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
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
        builder.Services.AddSingleton<Faker>();
        builder.Services.AddTransient<RazorRenderer>();
        //TODO винести в окрему конфігурацію
        builder.Services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqSettings = context.GetRequiredService<IOptions<RabbitMqSettings>>().Value;
                cfg.Host(rabbitMqSettings.Host, rabbitMqSettings.VirtualHost, h =>
                {
                    h.Username(rabbitMqSettings.Username);
                    h.Password(rabbitMqSettings.Password);
                });

                //TODO подивитися чи можна це винести, щоб не прописувати це для кожного меседжа
                cfg.Message<EmailMessage>(config =>
                {
                    config.SetEntityName("SendEmailExchange.fanout");
                });
                
                //TODO подивитися чи можна це винести, щоб не прописувати це для кожного меседжа
                cfg.Publish<EmailMessage>(publishConfig =>
                {
                    publishConfig.ExchangeType = ExchangeType.Fanout;
                });
                
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }   
}       