using Bogus;
using y_nuget;
using y_nuget.RabbitMq;
using YprojectUserService.Razor;
using YprojectUserService.Database;
using YprojectUserService.Authorization;
using YprojectUserService.RabbitMq;

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
        services.AddRabbitMqConfig(builder, x =>
        {
            x.AddConsumer<UserCategoryConsumerService>();
            x.UseInbox<ApplicationDbContext>();
        });
        
        services.AddYNugetConfiguration(builder);
        services.AddSingleton<Faker>();
        services.AddTransient<RazorRenderer>();

        return services;
    }   
}