using Microsoft.EntityFrameworkCore;

namespace YprojectUserService.Database;

public static class Configuration
{
    public static IServiceCollection AddDataBaseConfig(this IServiceCollection services
        ,IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        return services;
    }
}

public static class AutoMigration
{
    public static void AddAutoMigration(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            if(db != null)
            {
                if(db.Database.GetPendingMigrations().Any())
                {
                    db.Database.Migrate();
                }
            }
        }
    }
}