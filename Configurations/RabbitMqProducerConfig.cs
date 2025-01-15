using MassTransit;
using Microsoft.Extensions.Options;
using y_nuget.RabbitMq;
using YprojectUserService.Authorization.Commands.RegisterUser;

namespace YprojectUserService.Configurations;

public static class RabbitMqProducerConfig
{
    public static IServiceCollection AddRabbitMqProducerConfig(this IServiceCollection services,
        WebApplicationBuilder builder)
    {
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

                cfg.Message<EmailMessage>(config =>
                {
                    config.SetEntityName("email_templates_queue");
                });
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}